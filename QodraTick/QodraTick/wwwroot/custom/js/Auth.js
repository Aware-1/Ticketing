// cookie-auth.js - جاوا اسکریپت مدیریت کوکی و احراز هویت

class CookieAuthManager {
    constructor() {
        this.apiBaseUrl = '/api/auth';
        this.cookieName = 'QodraTickAuth';
        this.userDataKey = 'qodratick_user';
    }

    // ست کردن کوکی
    setCookie(name, value, days = 7) {
        const expires = new Date();
        expires.setTime(expires.getTime() + (days * 24 * 60 * 60 * 1000));
        document.cookie = `${name}=${value};expires=${expires.toUTCString()};path=/;SameSite=Lax`;
    }

    // دریافت کوکی
    getCookie(name) {
        const nameEQ = name + "=";
        const ca = document.cookie.split(';');
        for (let i = 0; i < ca.length; i++) {
            let c = ca[i];
            while (c.charAt(0) === ' ') c = c.substring(1, c.length);
            if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
        }
        return null;
    }

    // حذف کوکی
    deleteCookie(name) {
        document.cookie = `${name}=;expires=Thu, 01 Jan 1970 00:00:00 UTC;path=/;`;
    }

    // ذخیره اطلاعات کاربر در localStorage
    setUserData(userData) {
        localStorage.setItem(this.userDataKey, JSON.stringify(userData));
    }

    // دریافت اطلاعات کاربر
    getUserData() {
        const userData = localStorage.getItem(this.userDataKey);
        return userData ? JSON.parse(userData) : null;
    }

    // حذف اطلاعات کاربر
    clearUserData() {
        localStorage.removeItem(this.userDataKey);
    }

    // بررسی لاگین بودن
    isAuthenticated() {
        const cookie = this.getCookie(this.cookieName);
        const userData = this.getUserData();
        return cookie && userData;
    }

    // لاگین کاربر
    async login(username, password, rememberMe = false) {
        try {
            const response = await fetch(`${this.apiBaseUrl}/login`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                credentials: 'include', // برای ارسال کوکی‌ها
                body: JSON.stringify({
                    username: username,
                    password: password,
                    rememberMe: rememberMe
                })
            });

            const result = await response.json();

            if (response.ok && result.success) {
                // ذخیره اطلاعات کاربر
                this.setUserData(result.user);

                // نوتیفیکیشن موفقیت
                this.showNotification('ورود موفقیت‌آمیز', result.message, 'success');

                return { success: true, user: result.user };
            } else {
                throw new Error(result.message || 'خطا در ورود');
            }
        } catch (error) {
            this.showNotification('خطا در ورود', error.message, 'error');
            return { success: false, error: error.message };
        }
    }

    // خروج کاربر
    async logout() {
        try {
            const response = await fetch(`${this.apiBaseUrl}/logout`, {
                method: 'POST',
                credentials: 'include'
            });

            // پاک کردن اطلاعات محلی
            this.deleteCookie(this.cookieName);
            this.clearUserData();

            if (response.ok) {
                this.showNotification('خروج موفقیت‌آمیز', 'از سیستم خارج شدید', 'success');

                // ریدایرکت به صفحه لاگین
                setTimeout(() => {
                    window.location.href = '/login';
                }, 1000);

                return { success: true };
            } else {
                throw new Error('خطا در خروج از سیستم');
            }
        } catch (error) {
            // در صورت خطا، اطلاعات محلی را پاک کن
            this.deleteCookie(this.cookieName);
            this.clearUserData();

            this.showNotification('خطا در خروج', error.message, 'error');
            window.location.href = '/login';
            return { success: false, error: error.message };
        }
    }

    // بررسی نقش کاربر
    hasRole(role) {
        const userData = this.getUserData();
        return userData && userData.role === role;
    }

    // بررسی دسترسی
    hasAccess(requiredRoles) {
        const userData = this.getUserData();
        if (!userData) return false;

        if (Array.isArray(requiredRoles)) {
            return requiredRoles.includes(userData.role);
        }
        return userData.role === requiredRoles;
    }

    // نمایش نوتیفیکیشن
    showNotification(title, message, type = 'info') {
        // ایجاد المنت نوتیفیکیشن
        const notificationArea = this.getOrCreateNotificationArea();

        const alertClass = type === 'error' ? 'danger' : type;
        const iconClass = this.getIconClass(type);

        const notification = document.createElement('div');
        notification.className = `alert alert-${alertClass} alert-dismissible fade show`;
        notification.innerHTML = `
            <i class="bi ${iconClass} me-2"></i>
            <strong>${title}</strong><br>
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;

        notificationArea.appendChild(notification);

        // حذف خودکار بعد از 5 ثانیه
        setTimeout(() => {
            if (notification.parentNode) {
                notification.classList.remove('show');
                setTimeout(() => notification.remove(), 150);
            }
        }, 5000);
    }

    // دریافت یا ایجاد محل نوتیفیکیشن
    getOrCreateNotificationArea() {
        let notificationArea = document.getElementById('notification-area');
        if (!notificationArea) {
            notificationArea = document.createElement('div');
            notificationArea.id = 'notification-area';
            notificationArea.className = 'position-fixed top-0 end-0 p-3';
            notificationArea.style.zIndex = '9999';
            document.body.appendChild(notificationArea);
        }
        return notificationArea;
    }

    // دریافت آیکون بر اساس نوع
    getIconClass(type) {
        const icons = {
            success: 'bi-check-circle',
            error: 'bi-exclamation-triangle',
            warning: 'bi-exclamation-circle',
            info: 'bi-info-circle'
        };
        return icons[type] || icons.info;
    }

    // بررسی وضعیت احراز هویت و ریدایرکت
    checkAuthAndRedirect() {
        if (!this.isAuthenticated()) {
            window.location.href = '/login';
            return false;
        }
        return true;
    }

    // لاگین خودکار در صورت وجود کوکی
    async autoLogin() {
        if (this.isAuthenticated()) {
            try {
                // بررسی اعتبار توکن با سرور
                const response = await fetch('/api/auth/validate', {
                    method: 'GET',
                    credentials: 'include'
                });

                if (!response.ok) {
                    // توکن نامعتبر، خروج
                    this.logout();
                    return false;
                }

                return true;
            } catch (error) {
                console.error('خطا در بررسی اعتبار:', error);
                this.logout();
                return false;
            }
        }
        return false;
    }
}

// ایجاد نمونه سراسری
window.authManager = new CookieAuthManager();

// توابع سراسری برای استفاده آسان
window.login = async (username, password, rememberMe) => {
    return await window.authManager.login(username, password, rememberMe);
};

window.logout = async () => {
    return await window.authManager.logout();
};

window.isAuthenticated = () => {
    return window.authManager.isAuthenticated();
};

window.getCurrentUser = () => {
    return window.authManager.getUserData();
};

window.hasRole = (role) => {
    return window.authManager.hasRole(role);
};

window.checkAuth = () => {
    return window.authManager.checkAuthAndRedirect();
};

// بررسی خودکار در بارگذاری صفحه
document.addEventListener('DOMContentLoaded', async () => {
    // بررسی اعتبار لاگین در صفحات محافظت شده
    const protectedPaths = ['/tickets', '/support', '/admin', '/dashboard'];
    const currentPath = window.location.pathname;

    if (protectedPaths.some(path => currentPath.startsWith(path))) {
        if (!(await window.authManager.autoLogin())) {
            window.location.href = '/login';
        }
    }

    // اگر در صفحه لاگین هستیم و کاربر قبلاً لاگین کرده
    if (currentPath === '/login' && window.authManager.isAuthenticated()) {
        window.location.href = '/';
    }
});

// مدیریت خروج خودکار در صورت بستن مرورگر
window.addEventListener('beforeunload', () => {
    const userData = window.authManager.getUserData();
    if (userData && !userData.rememberMe) {
        window.authManager.logout();
    }
});

console.log('🔐 Cookie Auth Manager loaded successfully');