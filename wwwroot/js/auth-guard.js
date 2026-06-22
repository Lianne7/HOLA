// auth-guard.js - Route Protection & Authentication System for HOLA

class AuthGuard {
    constructor() {
        this.currentUser = null;
        this.init();
    }

    init() {
        // Load user from localStorage if exists
        const savedUser = localStorage.getItem('holaUser');
        if (savedUser) {
            this.currentUser = JSON.parse(savedUser);
        }
    }

    // Check if user is authenticated
    isAuthenticated() {
        return this.currentUser !== null;
    }

    // Get current user
    getCurrentUser() {
        return this.currentUser;
    }

    // Get user tier (student, educator, admin)
    getUserTier() {
        return this.currentUser?.userTier || null;
    }

    // Login user
    login(userData) {
        this.currentUser = userData;
        localStorage.setItem('holaUser', JSON.stringify(userData));
        return true;
    }

    // Logout user
    logout() {
        this.currentUser = null;
        localStorage.removeItem('holaUser');
        window.location.href = 'login.html';
    }

    // Protect route - redirect if not authenticated
    requireAuth() {
        if (!this.isAuthenticated()) {
            window.location.href = 'login.html';
            return false;
        }
        return true;
    }

    // Redirect if already authenticated
    requireGuest() {
        if (this.isAuthenticated()) {
            this.redirectToDashboard();
            return false;
        }
        return true;
    }

    // Require specific user tier
    requireTier(allowedTiers) {
        if (!this.isAuthenticated()) {
            window.location.href = 'login.html';
            return false;
        }

        const userTier = this.getUserTier();
        if (!allowedTiers.includes(userTier)) {
            alert('Access denied. You do not have permission to view this page.');
            this.redirectToDashboard();
            return false;
        }

        return true;
    }

    // Redirect to appropriate dashboard based on user tier
    redirectToDashboard() {
        const tier = this.getUserTier();

        switch (tier) {
            case 'student':
                window.location.href = 'dashboard-student.html';
                break;
            case 'educator':
                window.location.href = 'dashboard-educator.html';
                break;
            case 'admin':
                window.location.href = 'dashboard-admin.html';
                break;
            default:
                window.location.href = 'login.html';
        }
    }

    // Get dashboard URL for current user
    getDashboardUrl() {
        const tier = this.getUserTier();

        switch (tier) {
            case 'student':
                return 'dashboard-student.html';
            case 'educator':
                return 'dashboard-educator.html';
            case 'admin':
                return 'dashboard-admin.html';
            default:
                return 'login.html';
        }
    }
}

// Create global instance
const authGuard = new AuthGuard();

// Export for use in other files
if (typeof module !== 'undefined' && module.exports) {
    module.exports = AuthGuard;
}