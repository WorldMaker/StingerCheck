﻿import ko = require('knockout');
export import router = require("plugins/router");
import security = require('../models/security');
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
            $.post('/Token', { grant_type: 'persona', assertion: assertion }).then(result => result, util.failAsJson).fail(err => {
                toastr.error(err.error_description || err.error || "Unknown error occurred", "Login Failure");
                navigator.id.logout(); // avoid loops
            }).then(result => {
                if (result.access_token) {
                    security.setAccessToken(result.access_token, false);
                }
                user(result);
            });
        },
        onlogout: () => {
            $.post('/api/Persona/logout').then(result => result, util.failAsJson).fail(err => {
                toastr.error(err.message || "An unknown error occured.", "Logout Failure");
            }).always(() => {
                security.clearAccessToken();
                user(null);
            });
        },
    });

    router.map([
        { route: '', title: 'Now Playing', moduleId: 'viewmodels/welcome', nav: true },
        { route: 'about', title: 'About', moduleId: 'viewmodels/about', nav: true },
        { route: 'detail/:tomatoId', title: 'Movie Details', moduleId: 'viewmodels/detail', nav: false },
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