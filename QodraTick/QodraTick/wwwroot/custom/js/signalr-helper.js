let hubConnection = null;
let dotnetRef = null;

// Initialize SignalR connection for ticket hub
window.initializeTicketHub = async (dotNetReference, ticketId) => {
    try {
        if (hubConnection) {
            await hubConnection.stop();
        }

        dotnetRef = dotNetReference;

        // Create connection
        hubConnection = new signalR.HubConnectionBuilder()
            .withUrl("/ticketHub")
            .withAutomaticReconnect([0, 2000, 10000, 30000])
            .build();

        // Handle connection events
        hubConnection.onreconnecting(() => {
            console.log("SignalR reconnecting...");
            showConnectionStatus("در حال اتصال مجدد...", "warning");
        });

        hubConnection.onreconnected(() => {
            console.log("SignalR reconnected");
            showConnectionStatus("متصل شد", "success");
            if (ticketId) {
                hubConnection.invoke("JoinTicket", ticketId.toString());
            }
        });

        hubConnection.onclose(() => {
            console.log("SignalR connection closed");
            showConnectionStatus("قطع شد", "danger");
        });

        // Handle incoming messages
        hubConnection.on("ReceiveMessage", (message) => {
            if (dotnetRef) {
                dotnetRef.invokeMethodAsync('OnMessageReceived', message);
            }
            addMessageToChat(message);
        });

        // Handle notifications
        hubConnection.on("ReceiveNotification", (notification) => {
            showNotification(notification.title, notification.message, notification.type);

            // Play notification sound (optional)
            playNotificationSound();
        });

        // Handle ticket updates
        hubConnection.on("TicketAssigned", (data) => {
            if (dotnetRef) {
                dotnetRef.invokeMethodAsync('OnTicketUpdated');
            }
        });

        hubConnection.on("RefreshTicketList", () => {
            // Refresh ticket lists in support pages
            if (window.location.pathname.includes('/support/')) {
                location.reload();
            }
        });

        hubConnection.on("RefreshDashboard", () => {
            // Refresh admin dashboard
            if (window.location.pathname.includes('/admin/') || window.location.pathname === '/') {
                location.reload();
            }
        });

        // Handle errors
        hubConnection.on("Error", (error) => {
            showToast(error, "error");
        });

        // Start connection
        await hubConnection.start();
        console.log("SignalR Connected");
        showConnectionStatus("متصل", "success");

        // Join ticket room if specified
        if (ticketId) {
            await hubConnection.invoke("JoinTicket", ticketId.toString());
        }

        return hubConnection;

    } catch (error) {
        console.error("SignalR Connection Error:", error);
        showConnectionStatus("خطا در اتصال", "danger");
        throw error;
    }
};

// Add message to chat UI
function addMessageToChat(message) {
    const container = document.getElementById('messages-container');
    if (!container) return;

    const messageDiv = document.createElement('div');
    messageDiv.className = `mb-3 ${message.isFromSupport ? 'text-start' : 'text-end'}`;

    const bubbleClass = message.isFromSupport ? 'bg-light' : 'bg-primary text-white';
    const timeClass = message.isFromSupport ? 'text-muted' : 'text-white-50';

    messageDiv.innerHTML = `
        <div class="d-inline-block p-2 rounded ${bubbleClass}" style="max-width: 70%;">
            <div>${message.content}</div>
            <small class="${timeClass}">
                ${message.userName} - ${message.createdAt}
            </small>
        </div>
    `;

    container.appendChild(messageDiv);
    scrollToBottom('messages-container');
}

// Show connection status
function showConnectionStatus(message, type) {
    const statusDiv = document.getElementById('connection-status');
    if (statusDiv) {
        statusDiv.className = `alert alert-${type} alert-sm`;
        statusDiv.textContent = `اتصال: ${message}`;

        // Auto hide success messages
        if (type === 'success') {
            setTimeout(() => {
                statusDiv.style.display = 'none';
            }, 3000);
        } else {
            statusDiv.style.display = 'block';
        }
    }
}

// Show notification
function showNotification(title, message, type = 'info') {
    // Create notification element if it doesn't exist
    let notificationArea = document.getElementById('notification-area');
    if (!notificationArea) {
        notificationArea = document.createElement('div');
        notificationArea.id = 'notification-area';
        notificationArea.className = 'position-fixed top-0 end-0 p-3';
        notificationArea.style.zIndex = '9999';
        document.body.appendChild(notificationArea);
    }

    const alertClass = type === 'error' ? 'danger' : type;
    const notification = document.createElement('div');
    notification.className = `alert alert-${alertClass} alert-dismissible fade show`;
    notification.innerHTML = `
        <strong>${title}</strong><br>
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
    `;

    notificationArea.appendChild(notification);

    // Auto remove after 5 seconds
    setTimeout(() => {
        if (notification.parentNode) {
            notification.classList.remove('show');
            setTimeout(() => {
                notification.remove();
            }, 150);
        }
    }, 5000);
}

// Play notification sound
function playNotificationSound() {
    try {
        // Create a simple notification sound
        const audioContext = new (window.AudioContext || window.webkitAudioContext)();
        const oscillator = audioContext.createOscillator();
        const gainNode = audioContext.createGain();

        oscillator.connect(gainNode);
        gainNode.connect(audioContext.destination);

        oscillator.frequency.setValueAtTime(800, audioContext.currentTime);
        oscillator.frequency.setValueAtTime(600, audioContext.currentTime + 0.1);

        gainNode.gain.setValueAtTime(0, audioContext.currentTime);
        gainNode.gain.linearRampToValueAtTime(0.3, audioContext.currentTime + 0.01);
        gainNode.gain.exponentialRampToValueAtTime(0.01, audioContext.currentTime + 0.2);

        oscillator.start(audioContext.currentTime);
        oscillator.stop(audioContext.currentTime + 0.2);
    } catch (error) {
        console.log("Could not play notification sound:", error);
    }
}

// Scroll to bottom helper
window.scrollToBottom = (elementId) => {
    const element = document.getElementById(elementId);
    if (element) {
        element.scrollTop = element.scrollHeight;
    }
};

// Show toast helper
window.showToast = (message, type = 'info') => {
    const toastElement = document.getElementById('notificationToast');
    const toastBody = document.getElementById('toastBody');

    if (toastElement && toastBody) {
        toastBody.textContent = message;

        // Update toast color based on type
        toastElement.className = 'toast';
        if (type === 'error') {
            toastElement.classList.add('border-danger');
        } else if (type === 'success') {
            toastElement.classList.add('border-success');
        }

        const toast = new bootstrap.Toast(toastElement);
        toast.show();
    }
};

// Cleanup on page unload
window.addEventListener('beforeunload', async () => {
    if (hubConnection) {
        try {
            await hubConnection.stop();
        } catch (error) {
            console.log("Error stopping SignalR connection:", error);
        }
    }
});

// Global SignalR functions for manual operations
window.signalR = {
    sendMessage: async (ticketId, message) => {
        if (hubConnection && hubConnection.state === signalR.HubConnectionState.Connected) {
            await hubConnection.invoke("SendMessage", ticketId, message);
        }
    },

    acceptTicket: async (ticketId) => {
        if (hubConnection && hubConnection.state === signalR.HubConnectionState.Connected) {
            await hubConnection.invoke("AcceptTicket", ticketId);
        }
    },

    closeTicket: async (ticketId) => {
        if (hubConnection && hubConnection.state === signalR.HubConnectionState.Connected) {
            await hubConnection.invoke("CloseTicket", ticketId);
        }
    },

    reassignTicket: async (ticketId, newSupportUserId) => {
        if (hubConnection && hubConnection.state === signalR.HubConnectionState.Connected) {
            await hubConnection.invoke("ReassignTicket", ticketId, newSupportUserId);
        }
    },

    notifyNewTicket: async (ticketId, subject, priority) => {
        if (hubConnection && hubConnection.state === signalR.HubConnectionState.Connected) {
            await hubConnection.invoke("NotifyNewTicket", ticketId, subject, priority);
        }
    }
};