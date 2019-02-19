using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Xamarin.Forms;
using App1;
using Android.Provider;

[assembly: Dependency(typeof(App1.Droid.ContactHelper))]
namespace App1.Droid
{
    class ContactHelper:IContacts
    {
        public async Task<List<ContactLists>> GetDeviceContactsAsync()
        {
            ContactLists selectedContact = new ContactLists();
            List<ContactLists> contactList = new List<ContactLists>();

            var uri = ContactsContract.CommonDataKinds.Phone.ContentUri;
            //TODO - problem rturn phone instead of email
            string[] projection = { ContactsContract.Contacts.InterfaceConsts.Id, ContactsContract.Contacts.InterfaceConsts.DisplayName, ContactsContract.CommonDataKinds.Phone.Number, ContactsContract.CommonDataKinds.Email.Address};
            var cursor = Forms.Context.ContentResolver.Query(uri, projection, null, null, ContactsContract.Contacts.InterfaceConsts.DisplayName + " ASC");

            if (cursor.MoveToFirst())
            {
                do
                {
                    contactList.Add(new ContactLists()
                    {
                        DisplayName = cursor.GetString(cursor.GetColumnIndex(projection[1])),
                        ContactNumber = cursor.GetString(cursor.GetColumnIndex(projection[2])),
                        ContactEmail = cursor.GetString(cursor.GetColumnIndex(projection[3]))
                    });
                } while (cursor.MoveToNext());

                cursor.Close();
            }
            
            return contactList;

        }
        private object ManagedQuery(Android.Net.Uri uri, string[] projection, object p1, object p2, object p3)
        {
            throw new NotImplementedException();
        }
    }
}