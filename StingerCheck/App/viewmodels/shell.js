define(["require", "exports", 'knockout', "plugins/router", '../models/security', 'toastr', '../util', 'jquery'], function(require, exports, ko, router, security, toastr, util, $) {
    exports.router = router;

    exports.user = ko.observable(null);

    function activate() {
        util.subscribeProgress(exports.router.isNavigating);

        navigator.id.watch({
            loggedInUser: loadUser,
            onlogin: function (assertion) {
                $.post('/Token', { grant_type: 'persona', assertion: assertion }).then(function (result) {
                    return result;
                }, util.failAsJson).fail(function (err) {
                    toastr.error(err.error_description || err.error || "Unknown error occurred", "Login Failure");
                    navigator.id.logout(); // avoid loops
                }).then(function (result) {
                    if (result.access_token) {
                        security.setAccessToken(result.access_token, false);
                    }
                    exports.user(result);
                });
            },
            onlogout: function () {
                $.post('/api/Persona/logout').then(function (result) {
                    return result;
                }, util.failAsJson).fail(function (err) {
                    toastr.error(err.message || "An unknown error occured.", "Logout Failure");
                }).always(function () {
                    security.clearAccessToken();
                    exports.user(null);
                });
            }
        });

        exports.router.map([
            { route: '', title: 'Now Playing', moduleId: 'viewmodels/welcome', nav: true },
            { route: 'detail/:tomatoId', title: 'Movie Details', moduleId: 'viewmodels/detail', nav: false }
        ]).buildNavigationModel();

        return exports.router.activate();
    }
    exports.activate = activate;

    function compositionComplete() {
        // Can't use KO bindings for these because apparently magic happens in them
        $('.persona-login').click(function () {
            navigator.id.request();
        });
        $('.persona-logout').click(function () {
            navigator.id.logout();
        });
    }
    exports.compositionComplete = compositionComplete;
});
//# sourceMappingURL=shell.js.map
