using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVCApp.Models;
using NuGet.Protocol;
using System.Text.Json;

namespace MVCApp.Controllers
{
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
        [HttpGet]
        public ActionResult Index()
        {
            return View(_contacts);
        }

        // GET: ContactController/Details/5
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

        // GET: ContactController/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }

        // POST: ContactController/Create

        [HttpPost]
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

        // GET: ContactController/Edit/5
        [HttpGet]
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

        // POST: ContactController/Edit/5
        [HttpPost]
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

        // GET: ContactController/Delete/5
        [HttpGet]
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

        // POST: ContactController/Delete/5
        [HttpPost]
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
