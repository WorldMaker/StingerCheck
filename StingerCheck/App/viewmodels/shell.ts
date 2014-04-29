import ko = require('knockout');
export import router = require("plugins/router");
import toastr = require('toastr');
import util = require('../util');
import $ = require('jquery');

declare var loadUser: string;

export interface User {
    email: string;
}

export var user = ko.observable<User>(null);

export function activate() {
    util.subscribeProgress(router.isNavigating);

    navigator.id.watch({
        loggedInUser: loadUser,
        onlogin: assertion => {
            $.post('/api/Persona/login', { assertion: assertion }).then(result => result, util.failAsJson).fail(err => {
                toastr.error(err.reason, "Login Failure");
                navigator.id.logout(); // avoid loops
            }).then(result => user(result));
        },
        onlogout: () => {
            $.post('/api/Persona/logout').then(result => result, util.failAsJson).fail(err => {
                toastr.error(err.message || "An unknown error occured.", "Logout Failure");
            }).always(() => user(null));
        },
    });

    router.map([
        { route: '', title:'Welcome', moduleId: 'viewmodels/welcome', nav: true },
    ]).buildNavigationModel();
            
    return router.activate();
}

export function compositionComplete() {
    // Can't use KO bindings for these because apparently magic happens in them
    $('.persona-login').click(function () {
        navigator.id.request();
    });
    $('.persona-logout').click(function () {
        navigator.id.logout();
    });
}