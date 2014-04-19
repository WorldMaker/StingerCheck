define(["require", "exports", 'knockout', "plugins/router", 'toastr', '../util', 'jquery'], function(require, exports, ko, router, toastr, util, $) {
    exports.router = router;

    exports.user = ko.observable(null);

    function activate() {
        util.subscribeProgress(exports.router.isNavigating);

        navigator.id.watch({
            loggedInUser: loadUser,
            onlogin: function (assertion) {
                $.post('/api/Persona/login', { assertion: assertion }).then(function (result) {
                    return result;
                }, util.failAsJson).fail(function (err) {
                    toastr.error(err.reason, "Login Failure");
                    navigator.id.logout(); // avoid loops
                }).then(function (result) {
                    return exports.user(result);
                });
            },
            onlogout: function () {
                $.post('/api/Persona/logout').then(function (result) {
                    return result;
                }, util.failAsJson).fail(function (err) {
                    toastr.error(err.message || "An unknown error occured.", "Logout Failure");
                }).always(function () {
                    return exports.user(null);
                });
            }
        });

        exports.router.map([
            { route: '', title: 'Welcome', moduleId: 'viewmodels/welcome', nav: true },
            { route: 'flickr', moduleId: 'viewmodels/flickr', nav: true }
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
