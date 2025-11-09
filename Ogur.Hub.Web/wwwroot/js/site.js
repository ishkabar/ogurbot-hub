// Ogur.Hub Custom JavaScript

// Global configuration
window.OgurHub = {
    apiUrl: 'http://localhost:5000/api', // Will be overridden from server
    signalRUrl: 'http://localhost:5000/hubs/devices'
};

// Utility functions
const Utils = {
    formatDate: function(date) {
        return new Date(date).toLocaleString('pl-PL');
    },
    
    showNotification: function(message, type = 'info') {
        DevExpress.ui.notify({
            message: message,
            type: type,
            displayTime: 3000,
            position: {
                my: 'top right',
                at: 'top right',
                offset: '10 10'
            }
        });
    },
    
    showError: function(message) {
        this.showNotification(message, 'error');
    },
    
    showSuccess: function(message) {
        this.showNotification(message, 'success');
    }
};

// API Client
const ApiClient = {
    get: async function(endpoint) {
        try {
            const response = await fetch(`${window.OgurHub.apiUrl}${endpoint}`, {
                headers: {
                    'Authorization': `Bearer ${this.getToken()}`
                }
            });
            if (!response.ok) throw new Error('Request failed');
            return await response.json();
        } catch (error) {
            Utils.showError('Failed to fetch data');
            console.error(error);
            throw error;
        }
    },
    
    post: async function(endpoint, data = {}) {
        try {
            const response = await fetch(`${window.OgurHub.apiUrl}${endpoint}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${this.getToken()}`
                },
                body: JSON.stringify(data)
            });
            if (!response.ok) throw new Error('Request failed');
            return await response.json();
        } catch (error) {
            Utils.showError('Failed to save data');
            console.error(error);
            throw error;
        }
    },

    put: async function(endpoint, data) {
        try {
            const response = await fetch(`${window.OgurHub.apiUrl}${endpoint}`, {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json',
                    'Authorization': `Bearer ${this.getToken()}`
                },
                body: JSON.stringify(data)
            });
            if (!response.ok) throw new Error('Request failed');
            return await response.json();
        } catch (error) {
            Utils.showError('Failed to update data');
            console.error(error);
            throw error;
        }
    },

    delete: async function(endpoint) {
        try {
            const response = await fetch(`${window.OgurHub.apiUrl}${endpoint}`, {
                method: 'DELETE',
                headers: {
                    'Authorization': `Bearer ${this.getToken()}`
                }
            });
            if (!response.ok) throw new Error('Request failed');
            // DELETE might return 204 No Content
            if (response.status === 204) return null;
            return await response.json();
        } catch (error) {
            Utils.showError('Failed to delete data');
            console.error(error);
            throw error;
        }
    },
    
    getToken: function() {
        return localStorage.getItem('auth_token') || '';
    },
    
    setToken: function(token) {
        localStorage.setItem('auth_token', token);
    }
};

// SignalR Connection (for real-time updates)
const SignalRClient = {
    connection: null,
    
    init: async function() {
        // TODO: Initialize SignalR connection when needed
        console.log('SignalR ready');
    }
};

// Initialize on page load
$(document).ready(function() {
    console.log('Ogur.Hub initialized');
});
