define(["require", "exports", "plugins/router", '../util'], function(require, exports, router, util) {
    exports.router = router;

    function activate() {
        util.subscribeProgress(exports.router.isNavigating);

        exports.router.map([
            { route: '', title: 'Welcome', moduleId: 'viewmodels/welcome', nav: true },
            { route: 'flickr', moduleId: 'viewmodels/flickr', nav: true }
        ]).buildNavigationModel();

        return exports.router.activate();
    }
    exports.activate = activate;
});
//# sourceMappingURL=shell.js.map
