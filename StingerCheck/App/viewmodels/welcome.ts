import ko = require('knockout');
import movie = require('../models/movie');
import util = require('../util');
import $ = require('jquery');

class WelcomeVm {
    movies = ko.observableArray<movie.MovieVm>();

    activate() {
        this.movies.removeAll();
        return $.get('/api/Movies').then<movie.Movie[]>(result => result, util.failAsJson)
            .then(movies => ko.utils.arrayForEach(movies, m => this.movies.push(new movie.MovieVm(m)))); // TODO: cache for more than one activation?
    }
}

export = WelcomeVm;