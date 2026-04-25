public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IJwtService     _jwtService;

    public AuthService(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IJwtService jwtService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _jwtService     = jwtService;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        // Vérifier que les mots de passe correspondent
        if (request.Password != request.ConfirmPassword)
            throw new BadRequestException("Les mots de passe ne correspondent pas");

        // Vérifier unicité email
        if (await _userRepository.ExistsByEmailAsync(request.Email))
            throw new BadRequestException($"Email '{request.Email}' déjà utilisé");

        // Vérifier unicité username
        if (await _userRepository.ExistsByUsernameAsync(request.Username))
            throw new BadRequestException($"Username '{request.Username}' déjà utilisé");

        // Vérifier que le rôle existe
        var role = await _roleRepository.GetByIdAsync(request.RoleId)
            ?? throw new NotFoundException($"Rôle {request.RoleId} introuvable");

        // Créer l'utilisateur
        var user = new User
        {
            Firstname  = request.Firstname,
            Lastname   = request.Lastname,
            Email      = request.Email,
            Phone      = request.Phone,
            Username   = request.Username,
            Password   = BCrypt.Net.BCrypt.HashPassword(request.Password), // ← BCryptPasswordEncoder
            RoleId     = request.RoleId,
            IsEnabled  = true,
            AccountIsEnabled    = true,
            AccountIsNotExpired = true,
            AccountIsNotLocked  = true,
            CredentialNotExpired = true
        };

        await _userRepository.AddAsync(user);

        // Générer le token directement après register ← comme ton Spring
        var token = _jwtService.GenerateToken(user.Username, new List<string> { role.Name });

        return new AuthResponse(
            token.Token,
            token.ExpiresAt,
            user.Username,
            user.Firstname,
            user.Lastname,
            role.Name
        );
    }

    public async Task<AuthResponse> LoginAsync(AuthRequest request)
    {
        // Charger l'utilisateur avec son rôle ← loadUserByUsername()
        var user = await _userRepository.GetByUsernameAsync(request.Username)
            ?? throw new UnauthorizedAccessException("Identifiants invalides");

        // Vérifier le mot de passe ← BCryptPasswordEncoder.matches()
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            throw new UnauthorizedAccessException("Identifiants invalides");

        // Vérifier l'état du compte ← UserDetails checks
        if (!user.IsEnabled)
            throw new UnauthorizedAccessException("Compte désactivé");

        if (!user.AccountIsEnabled)
            throw new UnauthorizedAccessException("Compte bancaire désactivé");

        if (!user.AccountIsNotLocked)
            throw new UnauthorizedAccessException("Compte verrouillé");

        if (!user.AccountIsNotExpired)
            throw new UnauthorizedAccessException("Compte expiré");

        if (!user.CredentialNotExpired)
            throw new UnauthorizedAccessException("Credentials expirés");

        // Générer le token JWT ← jwtService.generateToken()
        var token = _jwtService.GenerateToken(
            user.Username,
            new List<string> { user.Role?.Name ?? "USER" }
        );

        return new AuthResponse(
            token.Token,
            token.ExpiresAt,
            user.Username,
            user.Firstname,
            user.Lastname,
            user.Role?.Name ?? "USER"
        );
    }
}