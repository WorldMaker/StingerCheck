import movie = require('../models/movie');
import util = require('../util');
import security = require('../models/security');
import shell = require('./shell');
import ko = require('knockout');
import toastr = require('toastr');

class StingerVm {
    user = shell.user;

    movie = ko.observable<movie.MovieVm>();

    hasMidStinger = ko.observable<boolean>();
    hasFinalStinger = ko.observable<boolean>();
    midTeaser = ko.observable<number>(0);
    midClosure = ko.observable<number>(0);
    midGag = ko.observable<number>(0);
    midEgg = ko.observable<number>(0);
    finalTeaser = ko.observable<number>(0);
    finalClosure = ko.observable<number>(0);
    finalGag = ko.observable<number>(0);
    finalEgg = ko.observable<number>(0);

    activate(tomatoId) {
        return $.get('/api/Movie/' + tomatoId).then<movie.Movie>(result => result, util.failAsJson)
            .then(m => this.movie(new movie.MovieVm(m)))
            .fail(e => toastr.error(e.message || "Could not load movie", "Movie Loading"));
    }

    vote() {
        $.ajax('/api/Stinger', {
            type: 'POST',
            headers: security.getSecurityHeaders(),
            data: {
                movie: {
                    id: this.movie().id(),
                    tomatoId: this.movie().tomatoId(),
                },
                hasMidStinger: this.hasMidStinger(),
                hasFinalStinger: this.hasFinalStinger(),
                midTeaser: this.midTeaser(),
                midClosure: this.midClosure(),
                midGag: this.midGag(),
                midEgg: this.midEgg(),
                finalTeaser: this.finalTeaser(),
                finalClosure: this.finalClosure(),
                finalGag: this.finalGag(),
                finalEgg: this.finalEgg(),
            },
        }).then(result => result, util.failAsJson)
            .fail(e => toastr.error(e.message || "Error posting vote", "Stinger Vote"))
            .then(m => {
                this.movie().update(m);
                toastr.success("Vote posted", "Stinger Vote");
            });
    }
}

export = StingerVm;