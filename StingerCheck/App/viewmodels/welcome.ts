import ko = require('knockout');
import movie = require('../models/movie');
import toastr = require('toastr');
import util = require('../util');
import $ = require('jquery');

class WelcomeVm {
    movies = ko.observableArray<movie.MovieVm>();

    activate() {
        this.movies.removeAll();
        return $.get('/api/Movie').then<movie.Movie[]>(result => result, util.failAsJson)
            .then(movies => ko.utils.arrayForEach(movies, m => this.movies.push(new movie.MovieVm(m))))
            .fail(data => toastr.error(data.message || "Failed to load Now Playing", "Now Playing")); // TODO: cache for more than one activation?
    }
}

export = WelcomeVm;