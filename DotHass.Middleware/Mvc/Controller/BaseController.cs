using DotHass.Middleware.Abstractions;
using DotHass.Middleware.Mvc.Action.Result;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace DotHass.Middleware.Mvc.Controller
{
    public abstract class BaseController : IMessageController
    {
        public OwinContext Context { get; set; }

        private string sid;

        public string Sid
        {
            get
            {
                if (String.IsNullOrEmpty(sid))
                {
                    this.sid = User.FindFirst(claim => claim.Type == ClaimTypes.Sid).Value;
                }
                return sid;
            }
            set
            {
                this.sid = value;
            }
        }

        public IOwinRequest Request => Context?.Request;

        public IOwinResponse Response => Context?.Response;

        public ClaimsPrincipal User => Context?.Get<ClaimsPrincipal>(OwinConstants.Security.User);
        public ISession Session => Context?.Get<ISession>("Middleware.Session");

        public virtual OkResult Ok()
         => new OkResult();

        public virtual OkObjectResult Ok(object value)
        => new OkObjectResult(value);

        public virtual StatusCodeResult Error(int statusCode)
        => new StatusCodeResult(statusCode);

        public virtual StatusCodeResult StatusCode(int statusCode)
        => new StatusCodeResult(statusCode);

        public virtual ContentResult Content(string content)
        => new ContentResult(content);

        public virtual ContentResult Error(int statusCode, string content)
        => Content(content, statusCode);

        public virtual ContentResult Content(string content, int statusCode)
        {
            var result = new ContentResult(content)
            {
                StatusCode = statusCode
            };
            return result;
        }

        /// <summary>
        /// Validates the specified <paramref name="model"/> instance.
        /// </summary>
        /// <param name="model">The model to validate.</param>
        /// <returns><c>true</c> if the <see cref="ModelState"/> is valid; <c>false</c> otherwise.</returns>

        public virtual bool TryValidateModel(
            object model,
            out ICollection<ValidationResult> results
            )
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(model, new ValidationContext(model), results, true);
        }

        #region dispose

        // Flag: Has Dispose already been called?
        private bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        //参数为true表示释放所有资源，只能由使用者调用
        //参数为false表示释放非托管资源，只能由垃圾回收器自动调用
        //如果子类有自己的非托管资源，可以重载这个函数，添加自己的非托管资源的释放
        //但是要记住，重载此函数必须保证调用基类的版本，以保证基类的资源正常释放
        protected virtual void Dispose(bool disposing)
        {
            //因為透過Dispose做資源的釋放後，資源仍舊會有短時間會滯留在記憶體中。此時若該物件仍具有強引用參考，垃圾收集器會無法對該物件做回收的動作。因此該物件有可能會因此再次被錯用 ，重覆釋放到已經釋放的資源，故需在該方法實作時做些檢查的動作。
            if (disposed)
                return;

            if (disposing)
            {
                // Free any other managed objects here.
                //释放由该类管理的对象.即该类创建的对象
            }

            // Free any unmanaged objects here.
            // 释放非托管资源
            disposed = true;
        }

        //由垃圾回收器调用，释放非托管资源
        //析构函数只能由垃圾回收器调用。
        ~BaseController()
        {
            Dispose(false);
        }

        #endregion dispose
    }
}