using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVCApp.Models;
using NuGet.Protocol;
using System.Text.Json;

namespace MVCApp.Controllers
{
    // This route attribute is used to define the route for the controller.
    // In this case it's kind of a goofy route name, but I'm doing it just
    // to show that any route name is legitimate.
    [Route("Contacting")]
    public class ContactsController : Controller
    {

        private List<Contact> _contacts;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // create constructor
        public ContactsController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            // Create a list of contacts
            _contacts = new();
            try
            {
                // interestingly, MVC/.Net Core does not have a built-in way to store Session state the
                // way that ASP.Net Framework does,
                // so we have to use the IHttpContextAccessor to get the Session object.
                // We are storing the contact list in the Session object.
                // Also interestingly, we beed to access this Session through the httpContextAccessor
                // object *initiall* (as shown below) but later in the code we can access it directly
                // through the HttpContext object. (Not sure why this is the case, but it is, LOL.)
                // Additionally, we can only store Strings and int's into an MVC Session (don't ask me:
                // I wasn't invited to the design meetings), so we've come up with a workaround to
                // store our *objects* and that is by serializing them into JSON strings, and then
                // storing those strings into Session.  When we want to access the objects we
                // deserialize the JSON strings back into objects. 

                _contacts = _httpContextAccessor.HttpContext.Session.GetString("Contacts").FromJson<List<Contact>>();
            }
            catch
            {
                CreateDummyContactList(ref _contacts);
                string? sContacts = JsonSerializer.Serialize(_contacts);
                _httpContextAccessor.HttpContext.Session.SetString("Contacts", sContacts);
            }
        }


        // GET: ContactController
        [HttpGet("Index")]
        public ActionResult Index()
        {
            return View(_contacts);
        }

        // I'm leaving this here purposely (below), just to show how that we can mix and match
        // traditional routing methodology (in this method) with
        // attribute routing (done for other action methods).
        // Note that the URL which is constructed by MVC is different from one methodology
        // to the other.  Interestingly, is we do *not* specify a route for this method at
        // the controller level, then the URL constructed by MVC for this action will be the same as the
        // URL constructed by the attribute routing methodology, i.e. it'll be "Contacting/{id}".
        // But in this case, since we are specifying a route for the controller,
        // the URL constructed by MVC for this action will be "Contacting?id={id}".
        // The end result is the same but there are two different ways to get there, and these
        // subtle differences in the URLs constructed by MVC are important to understand.
        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = _contacts.FirstOrDefault(m => m.ContactID == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        [HttpGet("Create")]
        public ActionResult Create()
        {
            return View();
        }


        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                // Create a new contact
                Contact contact = new Contact();
                contact!.FirstName = collection["FirstName"];
                contact!.LastName = collection["LastName"];
                contact!.Age = Convert.ToInt32(collection["Age"]);
                _contacts.Add(contact);
                // put the updated contact list back into the session
                HttpContext.Session.SetString("Contacts", JsonSerializer.Serialize(_contacts));

                return RedirectToAction(nameof(Index));
            }
            catch
            { }
            return RedirectToAction(nameof(Index));
        }

        [HttpGet("Edit/{id}")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = _contacts.Find(x => x.ContactID == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                // Find the contact which has been updated in the UI and update it in the data structure
                Contact? contact = _contacts.Find(x => x.ContactID == id);
                contact!.FirstName = collection["FirstName"];
                contact!.LastName = collection["LastName"];
                contact!.Age = Convert.ToInt32(collection["Age"]);

                // Now re-put the whole contact list back into the session
                HttpContext.Session.SetString("Contacts", JsonSerializer.Serialize(_contacts));

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        [HttpGet("Delete/{id}")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = _contacts.Find(x => x.ContactID == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            try
            {
                _contacts.Remove(_contacts.Find(x => x.ContactID == id));
                HttpContext.Session.SetString("Contacts", JsonSerializer.Serialize(_contacts));
            }
            catch
            {
            }
            return RedirectToAction(nameof(Index));
        }

        public void CreateDummyContactList(ref List<Contact> c)
        {
            // Create a list of contacts
            c.Add(new Contact { ContactID = 1, FirstName = "John1", LastName = "Doe1", Age = 21 });
            c.Add(new Contact { ContactID = 2, FirstName = "John2", LastName = "Doe2", Age = 22 });
            c.Add(new Contact { ContactID = 3, FirstName = "John3", LastName = "Doe3", Age = 23 });
            c.Add(new Contact { ContactID = 4, FirstName = "John4", LastName = "Doe4", Age = 24 });
            c.Add(new Contact { ContactID = 5, FirstName = "John5", LastName = "Doe5", Age = 25 });
            return;
        }

    }
}
