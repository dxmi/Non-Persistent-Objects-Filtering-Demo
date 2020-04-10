﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;

namespace NonPersistentObjectsDemo.Module.BusinessObjects {

    [VisibleInReports]
    [NavigationItem("Newsflash")]
    [DefaultProperty(nameof(Title))]
    [DevExpress.ExpressApp.ConditionalAppearance.Appearance("", Enabled = false, TargetItems = "*")]
    [DevExpress.ExpressApp.DC.DomainComponent]
    public class Article : NonPersistentObjectBase {
        internal Article() { }
        private int id;
        [Browsable(false)]
        [DevExpress.ExpressApp.Data.Key]
        public int ID {
            get { return id; }
            set { id = value; }
        }
        private Contact _Author;
        public Contact Author {
            get { return _Author; }
            set { SetPropertyValue(nameof(Author), ref _Author, value); }
        }
        private string _Title;
        public string Title {
            get { return _Title; }
            set { SetPropertyValue<string>(nameof(Title), ref _Title, value); }
        }
        private string _Content;
        [FieldSize(-1)]
        public string Content {
            get { return _Content; }
            set { SetPropertyValue<string>(nameof(Content), ref _Content, value); }
        }
    }

    class ArticleAdapter {
        private NonPersistentObjectSpace objectSpace;
        private static List<Article> articles;

        public ArticleAdapter(NonPersistentObjectSpace npos) {
            this.objectSpace = npos;
            objectSpace.ObjectsGetting += ObjectSpace_ObjectsGetting;
        }
        private void ObjectSpace_ObjectsGetting(object sender, ObjectsGettingEventArgs e) {
            if(e.ObjectType == typeof(Article)) {
                var collection = new DynamicCollection(objectSpace, e.ObjectType, e.Criteria, e.Sorting, e.InTransaction);
                collection.ObjectsGetting += DynamicCollection_ObjectsGetting;
                e.Objects = collection;
            }
        }
        private void DynamicCollection_ObjectsGetting(object sender, DynamicObjectsGettingEventArgs e) {
            if(e.ObjectType == typeof(Article)) {
                e.Objects = articles;
                e.ShapeRawData = true;
            }
        }

        static ArticleAdapter() {
            articles = new List<Article>();
            CreateDemoData();
        }

        #region DemoData
        static void CreateDemoData() {
            var gen = new GenHelper();
            var contacts = ContactAdapter.GetAllContacts();
            for(int i = 0; i < 5000; i++) {
                var id1 = gen.Next(contacts.Count);
                var id2 = gen.Next(contacts.Count - 1);
                articles.Add(new Article() {
                    ID = i,
                    Title = GenHelper.ToTitle(gen.MakeBlah(gen.Next(7))),
                    Content = gen.MakeBlahBlahBlah(5 + gen.Next(100), 7),
                    Author = contacts[id1]
                });
            }
        }
        #endregion
    }
}
