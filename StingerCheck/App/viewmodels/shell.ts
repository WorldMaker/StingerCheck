export import router = require("plugins/router");
import util = require('../util');

export function activate() {
    util.subscribeProgress(router.isNavigating);

    router.map([
        { route: '', title:'Welcome', moduleId: 'viewmodels/welcome', nav: true },
        { route: 'flickr', moduleId: 'viewmodels/flickr', nav: true }
    ]).buildNavigationModel();
            
    return router.activate();
}