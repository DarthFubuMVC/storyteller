using System;
using System.Collections.Generic;
using NUnit.Framework;
using StoryTeller.Assertions;
using StoryTeller.Domain;
using StoryTeller.Engine;

namespace StoryTeller.Testing.Engine
{
    public class Service
    {
        private readonly string _name;

        public Service(string name)
        {
            _name = name;
        }

        public string Name { get { return _name; } }

        public bool Equals(Service obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj._name, _name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (Service)) return false;
            return Equals((Service) obj);
        }

        public override int GetHashCode()
        {
            return (_name != null ? _name.GetHashCode() : 0);
        }
    }

    [TestFixture]
    public class ObjectFinderTester
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            finder = new ObjectFinder();
        }

        #endregion

        private ObjectFinder finder;

        [Test]
        public void array_of_non_simple_type_that_does_not_have_a_finder_cannot_be_parsed()
        {
            finder.CanBeParsed(typeof (Service[])).ShouldBeFalse();
        }

        [Test]
        public void array_of_non_simple_type_that_has_a_finder_can_be_parsed()
        {
            finder.RegisterFinder(x => new Service(x));
            finder.CanBeParsed(typeof (Service[])).ShouldBeTrue();
        }

        [Test]
        public void enumerable_of_non_simple_type_that_does_not_have_a_finder_cannot_be_parsed()
        {
            finder.CanBeParsed(typeof (IEnumerable<Service>)).ShouldBeFalse();
        }

        [Test]
        public void enumerable_of_non_simple_type_that_has_a_finder_can_be_parsed()
        {
            finder.RegisterFinder(x => new Service(x));
            finder.CanBeParsed(typeof (IEnumerable<Service>)).ShouldBeTrue();
        }

        [Test]
        public void enumerable_of_non_simple_type_that_has_a_finder_can_be_resolved()
        {
            finder.RegisterFinder(x=> new Service(x));
            finder.FromString<IEnumerable<Service>>("Josh, Chad, Jeremy, Brandon").ShouldHaveTheSameElementsAs(new[]
            {
                new Service("Josh"), 
                new Service("Chad"), 
                new Service("Jeremy"), 
                new Service("Brandon")
            });
        }


        [Test]
        public void datetimes_can_be_parsed()
        {
            finder.CanBeParsed(typeof (DateTime)).ShouldBeTrue();
            finder.CanBeParsed(typeof (DateTime?)).ShouldBeTrue();
        }

        [Test]
        public void enumeration_can_be_parsed()
        {
            finder.CanBeParsed(typeof (DayOfWeek)).ShouldBeTrue();
            finder.CanBeParsed(typeof (CellStatus)).ShouldBeTrue();
        }

        [Test]
        public void get_date_time_for_day_and_time()
        {
            DateTime date = ObjectFinder.GetDateTime("Saturday 14:30");

            date.DayOfWeek.ShouldEqual(DayOfWeek.Saturday);
            date.Date.AddHours(14).AddMinutes(30).ShouldEqual(date);
            (date >= DateTime.Today).ShouldBeTrue();
        }

        [Test]
        public void get_date_time_for_day_and_time_2()
        {
            DateTime date = ObjectFinder.GetDateTime("Monday 14:30");

            date.DayOfWeek.ShouldEqual(DayOfWeek.Monday);
            date.Date.AddHours(14).AddMinutes(30).ShouldEqual(date);
            (date >= DateTime.Today).ShouldBeTrue();
        }

        [Test]
        public void get_date_time_for_day_and_time_3()
        {
            DateTime date = ObjectFinder.GetDateTime("Wednesday 14:30");

            date.DayOfWeek.ShouldEqual(DayOfWeek.Wednesday);
            date.Date.AddHours(14).AddMinutes(30).ShouldEqual(date);
            (date >= DateTime.Today).ShouldBeTrue();
        }

        [Test]
        public void get_date_time_from_24_hour_time()
        {
            ObjectFinder.GetDateTime("14:30").ShouldEqual(DateTime.Today.AddHours(14).AddMinutes(30));
        }

        [Test]
        public void get_EMPTY_as_an_empty_array()
        {
            finder.FromString<int[]>(ObjectFinder.EMPTY)
                .ShouldBeOfType<int[]>()
                .Length.ShouldEqual(0);

            finder.FromString<string[]>(ObjectFinder.EMPTY)
                .ShouldBeOfType<string[]>()
                .Length.ShouldEqual(0);
        }

        [Test]
        public void get_enumeration_value()
        {
            finder.FromString<DayOfWeek>(DayOfWeek.Monday.ToString())
                .ShouldEqual(DayOfWeek.Monday);
        }

        [Test]
        public void get_nullable_enumeration_value()
        {
            finder.FromString<DayOfWeek?>(DayOfWeek.Monday.ToString())
                .ShouldEqual(DayOfWeek.Monday);
            finder.FromString<DayOfWeek?>("NULL").ShouldBeNull();
        }

        [Test]
        public void get_string_enumerable()
        {
            finder.FromString<IEnumerable<string>>("Josh, Chad, Jeremy, Brandon").ShouldHaveTheSameElementsAs(
                new[]
                    {"Josh", "Chad", "Jeremy", "Brandon"});
        }

        [Test]
        public void get_string_array()
        {
            finder.FromString<string[]>("Josh, Chad, Jeremy, Brandon")
                .ShouldEqual(new[] {"Josh", "Chad", "Jeremy", "Brandon"});
        }

        [Test]
        public void non_simple_type_that_does_not_have_a_finder_cannot_be_parsed()
        {
            finder.CanBeParsed(typeof (Service)).ShouldBeFalse();
        }

        [Test]
        public void non_simple_type_that_has_a_finder_can_be_parsed()
        {
            finder.RegisterFinder(x => new Service(x));
            finder.CanBeParsed(typeof (Service)).ShouldBeTrue();
        }

        [Test]
        public void nullable_enumeration_can_be_parsed()
        {
            finder.CanBeParsed(typeof (DayOfWeek?)).ShouldBeTrue();
        }

        [Test]
        public void nullables_can_be_parsed()
        {
            finder.CanBeParsed(typeof (int?)).ShouldBeTrue();
            finder.CanBeParsed(typeof (bool?)).ShouldBeTrue();
            finder.CanBeParsed(typeof (double?)).ShouldBeTrue();
        }

        [Test]
        public void parse_a_boolean()
        {
            finder.FromString<bool>("true").ShouldBeTrue();
            finder.FromString<bool>("True").ShouldBeTrue();
            finder.FromString<bool>("False").ShouldBeFalse();
            finder.FromString<bool>("false").ShouldBeFalse();
        }

        [Test]
        public void parse_a_nullable_boolean()
        {
            finder.FromString<bool?>("true").Value.ShouldBeTrue();
            finder.FromString<bool?>("True").Value.ShouldBeTrue();
            finder.FromString<bool?>("False").Value.ShouldBeFalse();
            finder.FromString<bool?>("false").Value.ShouldBeFalse();
            finder.FromString<bool?>("NULL").ShouldBeNull();
        }

        [Test]
        public void parsing_a_nullable_should_use_the_finder_for_the_inner_type()
        {
            finder.RegisterFinder<int>(text => 99);

            finder.FromString<int>("45").ShouldEqual(99);
            finder.FromString<int?>("32").ShouldEqual(99);
            finder.FromString<int?>("NULL").ShouldBeNull();
            finder.FromString<DateTime?>("TODAY").ShouldEqual(DateTime.Today);
        }

        [Test]
        public void parse_a_number()
        {
            finder.FromString<int>("32").ShouldEqual(32);
            finder.FromString<int>("-1").ShouldEqual(-1);

            finder.FromString<double>("123.45").ShouldEqual(123.45);
        }

        [Test]
        public void parse_a_string()
        {
            finder.FromString<string>("something").ShouldEqual("something");
            finder.FromString<string>(Step.BLANK).ShouldEqual(string.Empty);
            finder.FromString<string>(Step.NULL).ShouldBeNull();
        }

        [Test]
        public void parse_date()
        {
            finder.FromString<DateTime>("1/1/2009").ShouldEqual(new DateTime(2009, 1, 1));
        }

        [Test]
        public void parse_nullable_date()
        {
            finder.FromString<DateTime>("TODAY").ShouldEqual(DateTime.Today);
        }

        [Test]
        public void parse_timespan_as_days()
        {
            finder.FromString<TimeSpan>("5d").ShouldEqual(new TimeSpan(5, 0, 0, 0));
        }

        [Test]
        public void parse_timespan_as_hours()
        {
            finder.FromString<TimeSpan>("1.5 hours").ShouldEqual(new TimeSpan(1, 30, 0));
        }

        [Test]
        public void parse_timespan_as_minutes()
        {
            finder.FromString<TimeSpan>("  1   minute ").ShouldEqual(new TimeSpan(0, 1, 0));
        }

        [Test]
        public void parse_timespan_as_seconds()
        {
            finder.FromString<TimeSpan>("15 seconds").ShouldEqual(new TimeSpan(0, 0, 15));
        }

        [Test]
        public void parse_timespan_in_dotnet_format()
        {
            finder.FromString<TimeSpan>("3:5:2").ShouldEqual(new TimeSpan(3, 5, 2));
        }

        [Test]
        public void parse_timespan_with_unrecognized_units()
        {
            typeof (StorytellerAssertionException).ShouldBeThrownBy(() => finder.FromString<TimeSpan>("3 dais"));
        }

        [Test]
        public void parse_today()
        {
            finder.FromString<DateTime>("TODAY").ShouldEqual(DateTime.Today);
        }

        [Test]
        public void parse_today_minus_date()
        {
            finder.FromString<DateTime>("TODAY-3").ShouldEqual(DateTime.Today.AddDays(-3));
        }

        [Test]
        public void parse_today_plus_date()
        {
            finder.FromString<DateTime>("TODAY+5").ShouldEqual(DateTime.Today.AddDays(5));
        }

        [Test]
        public void primitive_array_can_be_parsed()
        {
            finder.CanBeParsed(typeof (string[])).ShouldBeTrue();
            finder.CanBeParsed(typeof (int[])).ShouldBeTrue();
            finder.CanBeParsed(typeof (bool?[])).ShouldBeTrue();
        }

        [Test]
        public void primitives_can_be_parsed()
        {
            finder.CanBeParsed(typeof (string)).ShouldBeTrue();
            finder.CanBeParsed(typeof (int)).ShouldBeTrue();
            finder.CanBeParsed(typeof (bool)).ShouldBeTrue();
        }

        [Test]
        public void register_and_retrieve_from_a_custom_finder_method()
        {
            finder.RegisterFinder(x => new Service(x));
            finder.FromString<Service>("Josh").Name.ShouldEqual("Josh");
        }

        [Test]
        public void register_and_retrieve_from_a_custom_finder_method_as_array()
        {
            finder.RegisterFinder(x => new Service(x));
            finder.FromString<Service[]>("Josh, Chad, Jeremy, Brandon")
                .ShouldEqual(new[]
                {
                    new Service("Josh"),
                    new Service("Chad"),
                    new Service("Jeremy"),
                    new Service("Brandon")
                });
        }

        [Test]
        public void testcontext_can_not_be_parsed()
        {
            finder.CanBeParsed(typeof (ITestContext)).ShouldBeFalse();
        }
    }
}