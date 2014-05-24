define(["require", "exports", 'knockout', '../models/movie', '../util', 'jquery'], function(require, exports, ko, movie, util, $) {
    var WelcomeVm = (function () {
        function WelcomeVm() {
            this.movies = ko.observableArray();
        }
        WelcomeVm.prototype.activate = function () {
            var _this = this;
            this.movies.removeAll();
            return $.get('/api/Movies').then(function (result) {
                return result;
            }, util.failAsJson).then(function (movies) {
                return ko.utils.arrayForEach(movies, function (m) {
                    return _this.movies.push(new movie.MovieVm(m));
                });
            });
        };
        return WelcomeVm;
    })();

    
    return WelcomeVm;
});
//# sourceMappingURL=welcome.js.map
