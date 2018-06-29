using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TheWall.Models;

namespace TheWall.Controllers
{
  public class HomeController : Controller
  {
    private readonly DataContext _context;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    public HomeController (DataContext context, UserManager<User> userManager,
      SignInManager<User> signInManager)
    {
      _context = context;
      _userManager = userManager;
      _signInManager = signInManager;

    }
    public IActionResult Index ()
    {
      return View ();
    }

    [HttpGet]
    [Route ("signin")]
    public IActionResult Login ()
    {
      return View ();
    }

    [HttpPost]
    [Route ("signin")]
    public async Task<IActionResult> Login (LoginViewModel model)
    {
      if (ModelState.IsValid)
      {
        User LoggingIn = _context.users.Where (u => u.UserName == model.UserName).SingleOrDefault ();
        // This doesn't count login failures towards account lockout
        // To enable password failures to trigger account lockout, set lockoutOnFailure: true
        var result = await _signInManager.PasswordSignInAsync (model.UserName, model.Password, false, false);
        if (result.Succeeded)
        {
          User LoggedIn = _context.users.SingleOrDefault (u => u.UserName == model.UserName);

          HttpContext.Session.SetString ("UserId", LoggedIn.Id);
          TempData["WelcomeMessage"] = $"Welcome, {LoggedIn.FirstName}";
          return RedirectToAction ("Messages");
        }
        else
        {
          ModelState.AddModelError (string.Empty, "Invalid login attempt.");
          TempData["PWError"] = "Invalid login attempt. Did you try logging in with an email address?";
          return View (model);
        }
      }

      // If we got this far, something failed, redisplay form
      return View (model);
    }

    [HttpGet]
    [Route ("register")]
    public IActionResult Register ()
    {
      return View ();
    }

    [HttpPost]
    [Route ("register")]
    public async Task<IActionResult> Register (RegisterViewModel model)
    {
      if (ModelState.IsValid)
      {
        //Create a new User object, without adding a Password
        User NewUser = new User
        {
          UserName = model.UserName,
          Email = model.Email,
          FirstName = model.FirstName,
          LastName = model.LastName,
          CreatedAt = DateTime.Now
        };
        //CreateAsync will attempt to create the User in the database, simultaneously hashing the
        //password
        IdentityResult result = await _userManager.CreateAsync (NewUser, model.Password);
        //If the User was added to the database successfully
        if (result.Succeeded)
        {
          HttpContext.Session.SetString ("UserId", NewUser.Id);
          //Sign In the newly created User
          //We're using the SignInManager, not the UserManager!
          await _signInManager.SignInAsync (NewUser, isPersistent : false);
          return RedirectToAction ("Messages");
        }
        //If the creation failed, add the errors to the View Model
        foreach (var error in result.Errors)
        {
          ModelState.AddModelError (string.Empty, error.Description);
          TempData["PWError"] = error.Description;
          return View ();
        }
      }
      return View ();
    }

    [HttpGet]
    [Route ("logout")]
    public async Task<IActionResult> LogOut ()
    {
      await _signInManager.SignOutAsync ();
      return RedirectToAction ("Index");
    }

    [HttpGet]
    [Route ("messages")]
    public IActionResult Messages ()
    {
      string UserId = HttpContext.Session.GetString ("UserId");
      ViewBag.LoggedIn = _context.users.SingleOrDefault (u => u.Id == UserId);

      if (ViewBag.LoggedIn == null)
      {
        return Redirect ("/signin");
      }

      List<Message> AllMessages = _context.Messages.Include (u => u.User).Include (c => c.Comments).ToList ();
      ViewBag.Messages = AllMessages.OrderByDescending (m => m.CreatedAt);

      return View ();
    }

    [HttpPost]
    [Route ("message")]
    public IActionResult Message (MessagingViewModel message, string UserId)
    {
      if (!ModelState.IsValid)
      {
        ViewBag.LoggedIn = _context.users.SingleOrDefault (u => u.Id == HttpContext.Session.GetString ("UserId"));
        TempData["MessageError"] = "Please enter an actual message.";
        return Redirect ("/messages");
      }
      else
      {
        Message NewMessage = new Message
        {
          MessageText = message.MessageText,
          User = _context.users.SingleOrDefault (user => user.Id == UserId)
        };
        _context.Add (NewMessage);
        _context.SaveChanges ();
        Message Created = _context.Messages.Last ();
        return Redirect ("/messages");
      }
    }

    [HttpPost]
    [Route ("comment")]
    public IActionResult Comment (MessagingViewModel comment, string UserId, int MessageId)
    {
      if (!ModelState.IsValid)
      {
        ViewBag.LoggedIn = _context.users.SingleOrDefault (u => u.Id == HttpContext.Session.GetString ("UserId"));
        TempData["CommentError"] = "Please enter an actual comment.";
        return Redirect ("/messages");
      }
      else
      {
        Comment NewComment = new Comment
        {
          CommentText = comment.CommentText,
          User = _context.users.SingleOrDefault (user => user.Id == UserId),
          MessageId = MessageId
        };
        _context.Add (NewComment);
        _context.SaveChanges ();
        Comment Created = _context.Comments.Last ();
        return Redirect ("/messages");
      }
    }

    [HttpGet]
    [Route ("/delete/{MessageId}")]
    public IActionResult Delete (int MessageId)
    {
      ViewBag.LoggedIn = _context.users.SingleOrDefault (u => u.Id == HttpContext.Session.GetString ("UserId"));
      Message delete = _context.Messages.SingleOrDefault (a => a.MessageId == MessageId);

      _context.Remove (delete);
      _context.SaveChanges ();
      return Redirect ("/messages");
    }
  }
}