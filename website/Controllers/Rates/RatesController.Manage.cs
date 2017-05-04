using Milton.Web.Mvc.Helpers;
using Milton.Website.Models.Rates;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace Milton.Website.Controllers.Rates
{
    public partial class RatesController
    {
        public ActionResult Manage(int hospitalId)
        {
            var hospital = _hospitalService.GetById(hospitalId);

            RateViewModel model = new RateViewModel()
            {
                BostwanaPrice = hospital.BotswanaPrice,
                BotswanaReturnPrice = hospital.BotswanaReturnPrice,
                HospitalId = hospital.HospitalId,
                SwazilandPrice = hospital.SwazilandPrice,
                SwazilandReturnPrice = hospital.SwazilandReturnPrice,
                Name = hospital.Name
            };

            ViewBag.Title = "Manage";
            return View(model);
        }

        [HttpPost]
        public ActionResult Manage(RateViewModel model)
        {
            if (model == null) return RedirectToAction("Index");

            //Map the values to the domain model
            AutoMapper.Mapper.CreateMap<RateViewModel, Milton.Database.Models.Medical.Hospital>();
            Milton.Database.Models.Medical.Hospital hospital = AutoMapper.Mapper.Map<RateViewModel, Milton.Database.Models.Medical.Hospital>(model);

            var ctx = Users.UserContext();

            hospital.ModifiedByUserId = ctx.Person.PersonId;

            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    //Create
                    _hospitalService.Insert(hospital);

                    //Commit
                    scope.Complete();
                }
                catch (DbEntityValidationException valExc)
                {

                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("Manage", ex);
                    return View(model);
                }
            }

            AddNotification("The hospital rates was updated successfully!", true);
            //Return to view
            if (!Request.IsAjaxRequest()) return RedirectToAction("Index");

            //Return Json
            model.HospitalId = hospital.HospitalId;
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}