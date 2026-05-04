// Point d'entrée de l'application — crée le builder qui va configurer tous les services
var builder = WebApplication.CreateBuilder(args);

// ── CORS ────────────────────────────────────────────────────────────────────
// Autorise le frontend (Angular :4200, React :3000) à appeler l'API
// Sans ça, le navigateur bloque toutes les requêtes cross-origin
// ← équivalent de corsConfigurationSource() + cors() dans Spring Security
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
        policy
            .WithOrigins("http://localhost:4200", "http://localhost:3000")
            .AllowAnyMethod()       // GET, POST, PUT, DELETE, OPTIONS
            .AllowAnyHeader()       // Authorization, Content-Type, etc.
            .AllowCredentials()     // autorise les cookies et headers d'auth
            .SetPreflightMaxAge(TimeSpan.FromSeconds(3600))); // cache la réponse OPTIONS 1h
});

// ── AUTHENTIFICATION JWT ─────────────────────────────────────────────────────
// Enregistre le système d'authentification avec JWT comme schéma par défaut
// ← équivalent de @EnableWebSecurity + SecurityFilterChain dans Spring Security
builder.Services
    .AddAuthentication(options =>
    {
        // Schéma utilisé pour vérifier l'identité sur chaque requête
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        // Schéma utilisé quand une ressource protégée est accédée sans auth → 401
        options.DefaultChallengeScheme    = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,  // vérifie que le token vient bien de notre API
            ValidateAudience         = true,  // vérifie que le token est destiné à notre app
            ValidateLifetime         = true,  // vérifie que le token n'est pas expiré
            ValidateIssuerSigningKey = true,  // vérifie la signature avec notre secret
            ValidIssuer              = builder.Configuration["Jwt:Issuer"],   // valeur dans appsettings.json
            ValidAudience            = builder.Configuration["Jwt:Audience"], // valeur dans appsettings.json
            IssuerSigningKey         = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)) // clé de signature HMAC-SHA256
        };
    });

// Enregistre le système d'autorisation (gestion des rôles et policies)
// ← équivalent de @EnableMethodSecurity + authorizeHttpRequests()
builder.Services.AddAuthorization(options =>
{
    // Toutes les routes nécessitent auth sauf [AllowAnonymous]
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});

// ── INFRASTRUCTURE ───────────────────────────────────────────────────────────
// Permet d'accéder au HttpContext depuis n'importe quel service injecté
// ← utilisé dans AppDbContext pour récupérer l'utilisateur connecté (@PrePersist)
builder.Services.AddHttpContextAccessor();

// Enregistre EF Core avec PostgreSQL comme provider de base de données
// ← équivalent de spring.datasource + spring.jpa dans application.yml
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// ── REPOSITORIES ─────────────────────────────────────────────────────────────
// Scoped = une instance par requête HTTP ← équivalent de @Repository
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IOperationRepository, OperationRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// ── SERVICES ─────────────────────────────────────────────────────────────────
// Scoped = une instance par requête HTTP ← équivalent de @Service
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IOperationService, OperationService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>(); // ← service JWT
builder.Services.AddScoped<IAuthService, AuthService>();

// ── GESTION DES EXCEPTIONS ───────────────────────────────────────────────────
// Enregistre le handler global d'exceptions ← équivalent de @ControllerAdvice
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
// Active le format RFC 7807 (ProblemDetails) pour les réponses d'erreur JSON
builder.Services.AddProblemDetails();

// ── CONTROLLERS + API ─────────────────────────────────────────────────────────
// Scanne et enregistre tous les controllers ← équivalent de @ComponentScan
builder.Services.AddControllers();
// Génère les métadonnées des endpoints pour la doc API
builder.Services.AddEndpointsApiExplorer();
// Active le support OpenAPI natif .NET 10
builder.Services.AddOpenApi();

// ── CONSTRUCTION DE L'APPLICATION ────────────────────────────────────────────
// Finalise la configuration et crée l'application
// Après ce point, on ne peut plus ajouter de services
var app = builder.Build();
app.MapGet("/", () => "Hello World!");
// ════════════════════════════════════════════════════════════════════════════
// PIPELINE DES MIDDLEWARES — L'ORDRE EST CRITIQUE
// Chaque requête HTTP traverse ces middlewares dans cet ordre exact
// ← équivalent de l'ordre des filtres dans SecurityFilterChain Spring
// ════════════════════════════════════════════════════════════════════════════

// 1. Gestion des exceptions — doit être en premier pour capturer
//    toutes les erreurs des middlewares suivants
//    ← équivalent de @ControllerAdvice appliqué globalement
app.UseExceptionHandler();

// 2. Documentation API — accessible sans authentification
//    → http://localhost:PORT/openapi/v1.json
app.MapOpenApi().AllowAnonymous();

// 3. Interface Scalar — remplace Swagger UI, plus moderne
//    → http://localhost:PORT/scalar/v1
app.MapScalarApiReference(options =>
{
    options.Title             = "Ebank API";
    options.Theme             = ScalarTheme.Purple;
    options.DefaultHttpClient = new(ScalarTarget.CSharp, ScalarClient.HttpClient);
}).AllowAnonymous();

// 4. CORS — doit être avant Authentication et Authorization
//    pour que les requêtes OPTIONS du preflight passent sans auth
//    ← équivalent de cors() dans Spring Security
app.UseCors("AllowFrontend");

// 5. Redirection HTTPS — force le protocole sécurisé
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection(); // ← uniquement en production
}

// 6. Authentication — lit le token JWT et identifie l'utilisateur
//    DOIT être avant UseAuthorization
//    ← équivalent de addFilterBefore(authFilter, ...)
app.UseAuthentication();

// 7. Authorization — vérifie si l'utilisateur a le droit d'accéder à la ressource
//    ← équivalent de authorizeHttpRequests() + @Secured
app.UseAuthorization();

// 8. Routing vers les controllers — en dernier
//    ← équivalent du scan des @RestController
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await context.Database.MigrateAsync();   // ← applique les migrations au démarrage
    await DataSeeder.SeedAsync(context);     // ← insère les rôles par défaut
}

// Démarre le serveur et écoute les requêtes
app.Run();