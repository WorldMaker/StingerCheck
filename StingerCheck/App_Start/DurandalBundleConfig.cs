using System;
using System.Web.Optimization;

namespace StingerCheck {
  public class DurandalBundleConfig {
    public static void RegisterBundles(BundleCollection bundles) {
      bundles.Add(
        new StyleBundle("~/Content/css")
          .Include("~/Content/dsplynrnt.bootstrap.min.css")
          .Include("~/Content/font-awesome.css")
          .Include("~/Content/starterkit.css")
          .Include("~/Content/nprogress.css")
          .Include("~/Content/toastr.css")
        );
    }
  }
}