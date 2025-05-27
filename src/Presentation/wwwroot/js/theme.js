// Check for theme preference when page loads
function initializeTheme() {
    // Check if theme is stored in localStorage
    const storedTheme = localStorage.getItem('theme');
    
    // Check for OS-level preference if no stored theme
    const prefersDarkScheme = window.matchMedia('(prefers-color-scheme: dark)').matches;
    
    // Set theme based on stored preference or OS preference
    if (storedTheme === 'dark' || (!storedTheme && prefersDarkScheme)) {
        document.documentElement.classList.add('dark');
        localStorage.setItem('theme', 'dark');
    } else {
        document.documentElement.classList.remove('dark');
        localStorage.setItem('theme', 'light');
    }
}

// Toggle between light and dark themes
function toggleTheme() {
    if (document.documentElement.classList.contains('dark')) {
        document.documentElement.classList.remove('dark');
        localStorage.setItem('theme', 'light');
    } else {
        document.documentElement.classList.add('dark');
        localStorage.setItem('theme', 'dark');
    }
    
    // Re-initialize Flowbite components if needed
    if (typeof window.initFlowbite === 'function') {
        window.initFlowbite();
    }
}

// DOMContentLoaded to initialize theme when page loads
document.addEventListener('DOMContentLoaded', initializeTheme);

// Expose functions to window for Blazor interop
window.themeManager = {
    initialize: initializeTheme,
    toggle: toggleTheme,
    setDarkMode: function(isDark) {
        if (isDark) {
            document.documentElement.classList.add('dark');
            localStorage.setItem('theme', 'dark');
        } else {
            document.documentElement.classList.remove('dark');
            localStorage.setItem('theme', 'light');
        }
        
        // Re-initialize Flowbite components
        if (typeof window.initFlowbite === 'function') {
            window.initFlowbite();
        }
    },
    isDarkMode: function() {
        return document.documentElement.classList.contains('dark');
    }
};