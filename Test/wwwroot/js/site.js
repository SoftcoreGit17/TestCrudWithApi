// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function isTokenExpired(token) {
    if (!token) return true;

    const parts = token.split('.');
    if (parts.length !== 3) return true; // Not a valid JWT

    const payloadBase64Url = parts[1];

    // Base64Url decode function
    function base64UrlDecode(str) {
        // Replace URL-safe chars
        str = str.replace(/-/g, '+').replace(/_/g, '/');
        // Pad with '=' to make length multiple of 4
        const pad = 4 - (str.length % 4);
        if (pad !== 4) {
            str += '='.repeat(pad);
        }
        return atob(str);
    }

    try {
        const payloadJson = base64UrlDecode(payloadBase64Url);
        const payload = JSON.parse(payloadJson);

        if (!payload.exp) return true; 

        const now = Math.floor(Date.now() / 1000);
        return payload.exp < now;
    } catch (e) {
        console.error("Invalid token format", e);
        return true;
    }
}

 function checkTokenOnLoad() {
    const token = localStorage.getItem("token");
    if (isTokenExpired(token)) {
        localStorage.removeItem("token");
        alert("Your session has expired. Please log in again.");
        window.location.href = "/Admin/Login";
    }
}

// then call this on each page:
document.addEventListener("DOMContentLoaded", () => {
    checkTokenOnLoad();
});
