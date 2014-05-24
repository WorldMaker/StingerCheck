import ko = require('knockout');

export interface Movie {
    id: number;
    tomatoId: string;
    title: string;
    tomatoUrl: string;
    hasMidStinger?: boolean;
    hasFinalStinger?: boolean;
    midTeaser?: number;
    midClosure?: number;
    midGag?: number;
    midEgg?: number;
    finalTeaser?: number;
    finalClosure?: number;
    finalGag?: number;
    finalEgg?: number;
}

export class MovieVm {
    id = ko.observable<number>();
    tomatoId = ko.observable<string>();
    title = ko.observable<string>();
    tomatoUrl = ko.observable<string>();
    hasMidStinger = ko.observable<boolean>();
    hasFinalStinger = ko.observable<boolean>();
    midTeaser = ko.observable<number>();
    midClosure = ko.observable<number>();
    midGag = ko.observable<number>();
    midEgg = ko.observable<number>();
    finalTeaser = ko.observable<number>();
    finalClosure = ko.observable<number>();
    finalGag = ko.observable<number>();
    finalEgg = ko.observable<number>();

    midType = ko.computed(() => {
        if (this.midClosure() == 3) return "closure";
        else if (this.midEgg() == 3) return "egg";
        else if (this.midGag() == 3) return "gag";
        else if (this.midTeaser() == 3) return "teaser";
        else return "undecided"; // TODO: Ties?
    });

    finalType = ko.computed(() => {
        if (this.finalClosure() == 3) return "closure";
        else if (this.finalEgg() == 3) return "egg";
        else if (this.finalGag() == 3) return "gag";
        else if (this.finalTeaser() == 3) return "teaser";
        else return "undecided"; // TODO: Ties?
    });

    constructor(movie: Movie) {
        this.update(movie);
    }

    update(movie: Movie) {
        this.id(movie.id);
        this.title(movie.title);
        this.tomatoId(movie.tomatoId);
        this.tomatoUrl(movie.tomatoUrl);
        this.hasFinalStinger(movie.hasFinalStinger || false);
        this.hasMidStinger(movie.hasMidStinger || false);
        this.midClosure(movie.midClosure || 0);
        this.midEgg(movie.midEgg || 0);
        this.midGag(movie.midGag || 0);
        this.midTeaser(movie.midTeaser || 0);
        this.finalClosure(movie.finalClosure || 0);
        this.finalEgg(movie.finalEgg || 0);
        this.finalGag(movie.finalGag || 0);
        this.finalTeaser(movie.finalTeaser || 0);
    }
}