export function getSecurityHeaders(): any {
    var accessToken = sessionStorage["accessToken"] || localStorage["accessToken"];

    if (accessToken) {
        return { "Authorization": "Bearer " + accessToken };
    }

    return {};
}

export function clearAccessToken() {
    localStorage.removeItem("accessToken");
    sessionStorage.removeItem("accessToken");
}

export function setAccessToken(accessToken, persistent) {
    if (persistent) {
        localStorage["accessToken"] = accessToken;
    } else {
        sessionStorage["accessToken"] = accessToken;
    }
}