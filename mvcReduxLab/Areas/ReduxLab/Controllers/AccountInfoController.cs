﻿using mvcReduxLab.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace mvcReduxLab.Areas.ReduxLab.Controllers
{
    public class AccountInfoController : Controller
    {
        // GET: ReduxLab/AccountInfo
        public ActionResult AppForm()
        {
            return View();
        }

        public ActionResult SaveFormData(AccountInfoFormDataVM formData)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new HttpStatusCodeResult(400, "CheckDataFaill");
                }

                using (var ctx = new MyDatabaseEntities())
                using (var txn = ctx.Database.BeginTransaction())
                {
                    //## 若已存在則更新，不存在則新增
                    var info = ctx.Account.Find(formData.accountInfo.name);
                    if (info != null)
                    {
                        //# 已存在則更新
                        //info.name = formData.accountInfo.name; // PK
                        info.email = formData.accountInfo.email;
                        info.mobilePhone = formData.accountInfo.mobilePhone;
                        info.birthday = formData.userInfo.birthday;
                        info.contactTime = formData.userInfo.contactTime;
                        info.remark = formData.userInfo.remark;

                        ctx.SaveChanges();
                        txn.Commit();
                    }
                    else
                    {
                        //# 不存在則新增
                        Account newInfo = new Account()
                        {
                            name = formData.accountInfo.name,
                            email = formData.accountInfo.email,
                            mobilePhone = formData.accountInfo.mobilePhone,
                            birthday = formData.userInfo.birthday,
                            contactTime = formData.userInfo.contactTime,
                            remark = formData.userInfo.remark
                        };

                        ctx.Account.Add(newInfo);
                        ctx.SaveChanges();
                        txn.Commit();
                    }
                }

                return new HttpStatusCodeResult(200);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, ex.Message);
            }
        }

        [HttpPost]
        public ActionResult LoadFormData(string name)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new HttpStatusCodeResult(400, "CheckDataFaill");
                }

                using (var ctx = new MyDatabaseEntities())
                {
                    var info = ctx.Account.SingleOrDefault(c => c.name == name);

                    if (info == null)
                        return new HttpStatusCodeResult(500, "無資料！");

                    AccountInfoFormDataVM formData = new AccountInfoFormDataVM()
                    {
                        accountInfo = new AccountInfoFormDataVM.AccountInfo
                        {
                            name = info.name,
                            email = info.email,
                            mobilePhone = info.mobilePhone
                        },
                        userInfo = new AccountInfoFormDataVM.UserInfo
                        {
                            birthday = info.birthday,
                            contactTime = info.contactTime,
                            remark = info.remark
                        }
                    };

                    return Json(formData);
                }
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(500, ex.Message);
            }
        }
    }
}