// Download file helper
window.downloadFile = function (filename, content) {
    const blob = new Blob([content], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);

    link.setAttribute('href', url);
    link.setAttribute('download', filename);
    link.style.visibility = 'hidden';

    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);

    URL.revokeObjectURL(url);
};

// Confirm dialog helper
window.confirm = function (message) {
    return window.confirm(message);
};

// Toast notification helper
window.showToast = function (message, type = 'success') {
    // You can integrate with Bootstrap Toast or other toast libraries
    console.log(`${type}: ${message}`);
    alert(message);
};

// Scroll to top helper
window.scrollToTop = function () {
    window.scrollTo({ top: 0, behavior: 'smooth' });
};

// Copy to clipboard helper
window.copyToClipboard = function (text) {
    return navigator.clipboard.writeText(text)
        .then(() => true)
        .catch(() => false);
};
