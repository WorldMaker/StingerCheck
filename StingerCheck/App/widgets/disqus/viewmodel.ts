declare var DISQUS: any;

class DisqusVm {
    identifier: string;
    title: string;

    activate(data) {
        this.identifier = data.identifier;
        this.title = data.title;
    }

    compositionComplete() {
        /* * * CONFIGURATION VARIABLES: EDIT BEFORE PASTING INTO YOUR WEBPAGE * * */
        var disqus_shortname = (<any>window).disqus_shortname = 'stingercheck'; // required: replace example with your forum shortname
        var disqus_identifier = (<any>window).disqus_identifier = this.identifier;
        var disqus_url = (<any>window).disqus_url = window.location.href;
        var disqus_title = (<any>window).disqus_title = this.title;

        // DISQUS reset pulled from StackOverflow
        //    http://stackoverflow.com/questions/21903875/same-disqus-thread-is-loading-for-all-articles
        //    https://stackoverflow.com/questions/8944287/disqus-loading-the-same-comments-for-dynamic-pages?rq=1
        if (typeof DISQUS != 'undefined') {
            DISQUS.reset({
                reload: true,
                config: function () {
                    this.page.identifier = disqus_identifier;
                    this.page.url = disqus_url;
                    this.page.title = disqus_title;
                }
            });
        } else {

            /* * * DON'T EDIT BELOW THIS LINE * * */
            (function () {
                var dsq = document.createElement('script'); dsq.type = 'text/javascript'; dsq.async = true;
                dsq.src = '//' + disqus_shortname + '.disqus.com/embed.js';
                (<any>document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0]).appendChild(dsq);
            })();

        }
    }
}

export = DisqusVm;