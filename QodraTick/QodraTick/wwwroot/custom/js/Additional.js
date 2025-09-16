
        // Global error handler
    window.addEventListener('error', function(e) {
        console.error('Global error:', e.error);

    // نمایش خطا به کاربر در صورت نیاز
    if (window.authManager) {
        window.authManager.showNotification('خطا', 'خطایی در سیستم رخ داد', 'error');
            }
        });

    // مدیریت قطع اتصال
    window.addEventListener('beforeunload', function() {
            // قطع اتصال SignalR
            if (window.ticketHubConnection) {
        window.ticketHubConnection.stop();
            }
        });

    // Auto-focus روی اولین ورودی
    document.addEventListener('DOMContentLoaded', function() {
        setTimeout(() => {
            const firstInput = document.querySelector('input[type="text"]:not([readonly]), input[type="email"]:not([readonly]), textarea:not([readonly])');
            if (firstInput && !firstInput.disabled) {
                firstInput.focus();
            }
        }, 100);
        });

    // توابع کمکی سراسری
    window.utilities = {
        // کپی متن به کلیپ‌برد
        copyToClipboard: async function(text) {
                try {
        await navigator.clipboard.writeText(text);
    if (window.authManager) {
        window.authManager.showNotification('موفق', 'متن کپی شد', 'success');
                    }
    return true;
                } catch (err) {
        console.error('خطا در کپی:', err);
    return false;
                }
            },

    // فرمت کردن تاریخ
    formatDateTime: function(dateString) {
                const date = new Date(dateString);
    const options = {
        year: 'numeric',
    month: '2-digit',
    day: '2-digit',
    hour: '2-digit',
    minute: '2-digit',
    hour12: false
                };
    return date.toLocaleDateString('fa-IR', options);
            },

    // محاسبه زمان گذشته
    timeAgo: function(dateString) {
                const now = new Date();
    const date = new Date(dateString);
    const diffInSeconds = Math.floor((now - date) / 1000);

    if (diffInSeconds < 60) return 'همین الان';
    if (diffInSeconds < 3600) return Math.floor(diffInSeconds / 60) + ' دقیقه پیش';
    if (diffInSeconds < 86400) return Math.floor(diffInSeconds / 3600) + ' ساعت پیش';
    if (diffInSeconds < 2592000) return Math.floor(diffInSeconds / 86400) + ' روز پیش';
    return Math.floor(diffInSeconds / 2592000) + ' ماه پیش';
            },

    // بارگذاری با تاخیر
    delay: function(ms) {
                return new Promise(resolve => setTimeout(resolve, ms));
            },

    // اعتبارسنجی ایمیل
    validateEmail: function(email) {
                const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
            },

    // تبدیل حجم فایل به فرمت قابل خواندن
    formatFileSize: function(bytes) {
                if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
            },

    // اسکرول به پایین
    scrollToBottom: function(elementId) {
                const element = document.getElementById(elementId);
    if (element) {
        element.scrollTop = element.scrollHeight;
                }
            },

    // لودینگ اورلی
    showLoading: function(message = 'در حال بارگذاری...') {
                const overlay = document.createElement('div');
    overlay.id = 'loading-overlay';
    overlay.className = 'loading-overlay';
    overlay.innerHTML = `
    <div class="text-center text-white">
        <div class="spinner-border mb-3" style="width: 3rem; height: 3rem;"></div>
        <p>${message}</p>
    </div>
    `;
    document.body.appendChild(overlay);
            },

    hideLoading: function() {
                const overlay = document.getElementById('loading-overlay');
    if (overlay) {
        overlay.remove();
                }
            }
        };

    // تنظیمات اولیه
    window.qodraTick = {
        config: {
        maxFileSize: 5 * 1024 * 1024, // 5MB
    allowedExtensions: ['.jpg', '.jpeg', '.png', '.gif', '.pdf', '.doc', '.docx', '.txt'],
    apiBaseUrl: '/api',
    signalRHubUrl: '/ticketHub'
            },

    // تابع بررسی پشتیبانی مرورگر
    checkBrowserSupport: function() {
                const features = {
        localStorage: !!window.localStorage,
    sessionStorage: !!window.sessionStorage,
    fetch: !!window.fetch,
    webSocket: !!window.WebSocket,
    notifications: !!window.Notification
                };

                const unsupported = Object.keys(features).filter(key => !features[key]);
                if (unsupported.length > 0) {
        console.warn('برخی ویژگی‌ها پشتیبانی نمی‌شوند:', unsupported);
                }

    return features;
            },

    // آماده‌سازی اولیه
    initialize: function() {
        console.log('🎫 QodraTick System initialized');

    // بررسی پشتیبانی مرورگر
    this.checkBrowserSupport();

    // تنظیم زبان صفحه
    document.documentElement.lang = 'fa';
    document.documentElement.dir = 'rtl';

    // تنظیم عنوان پیش‌فرض
    if (!document.title || document.title === '') {
        document.title = 'قدرا تیک - سیستم مدیریت تیکت';
                }
            }
        };

    // اجرای تنظیمات اولیه
    document.addEventListener('DOMContentLoaded', function() {
        window.qodraTick.initialize();
        });

    // مدیریت تغییر سایز صفحه
    window.addEventListener('resize', function() {
            // تنظیم مجدد TinyMCE در صورت نیاز
            if (window.tinymce) {
        window.tinymce.editors.forEach(editor => {
            editor.getContainer().style.width = '100%';
        });
            }
        });

    // مدیریت اتصال آنلاین/آفلاین
    window.addEventListener('online', function() {
            if (window.authManager) {
        window.authManager.showNotification('اتصال برقرار شد', 'دوباره به اینترنت متصل شدید', 'success');
            }
        });

    window.addEventListener('offline', function() {
            if (window.authManager) {
        window.authManager.showNotification('قطع اتصال', 'اتصال اینترنت قطع شده است', 'warning');
            }
        });

    // Console log برای debug
    console.log('🚀 QodraTick Application loaded successfully');
    console.log('📊 Environment:', document.location.hostname === 'localhost' ? 'Development' : 'Production');
