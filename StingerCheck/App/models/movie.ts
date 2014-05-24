import ko = require('knockout');

export interface Movie {
    Id: number;
    TomatoId: string;
    Title: string;
    TomatoUrl: string;
    HasMidStinger?: boolean;
    HasFinalStinger?: boolean;
    MidTeaser?: number;
    MidClosure?: number;
    MidGag?: number;
    MidEgg?: number;
    FinalTeaser?: number;
    FinalClosure?: number;
    FinalGag?: number;
    FinalEgg?: number;
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
        this.id(movie.Id);
        this.title(movie.Title);
        this.tomatoId(movie.TomatoId);
        this.tomatoUrl(movie.TomatoUrl);
        this.hasFinalStinger(movie.HasFinalStinger || false);
        this.hasMidStinger(movie.HasMidStinger || false);
        this.midClosure(movie.MidClosure || 0);
        this.midEgg(movie.MidEgg || 0);
        this.midGag(movie.MidGag || 0);
        this.midTeaser(movie.MidTeaser || 0);
        this.finalClosure(movie.FinalClosure || 0);
        this.finalEgg(movie.FinalEgg || 0);
        this.finalGag(movie.FinalGag || 0);
        this.finalTeaser(movie.FinalTeaser || 0);
    }
}