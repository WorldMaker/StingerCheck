define(["require", "exports", 'knockout', '../models/movie', 'toastr', '../util', 'jquery'], function(require, exports, ko, movie, toastr, util, $) {
    var WelcomeVm = (function () {
        function WelcomeVm() {
            this.movies = ko.observableArray();
        }
        WelcomeVm.prototype.activate = function () {
            var _this = this;
            this.movies.removeAll();
            return $.get('/api/Movie').then(function (result) {
                return result;
            }, util.failAsJson).then(function (movies) {
                return ko.utils.arrayForEach(movies, function (m) {
                    return _this.movies.push(new movie.MovieVm(m));
                });
            }).fail(function (data) {
                return toastr.error(data.message || "Failed to load Now Playing", "Now Playing");
            });
        };
        return WelcomeVm;
    })();

    
    return WelcomeVm;
});
//# sourceMappingURL=welcome.js.map
