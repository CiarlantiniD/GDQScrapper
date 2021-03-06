﻿using GDQScrapper.GDQProcessor.Domain.HTMLTableExtractor;
using GDQScrapper.HtmlDataExtractor.Domain.Exceptions;
using NUnit.Framework;

namespace Tests.HtmlExtractor
{
    public class HtmlEventExtractorServiceShould
    {
        private HtmlEventExtractorService htmlEventsExtractorService;

        private readonly string SomeStartDate = "<td>2021-01-03T10:30:00Z</td>";
        private readonly string SomeGameName = "<td>Game</td>";
        private readonly string SomeRunnerName = "<td>Runner</td>";
        private readonly string SomeSetupDuration = "<td>0:10:00</td>";
        private readonly string SomeEventDuration = "<td>0:20:00</td>";
        private readonly string SomeConditionAndPlatform = "<td>Condition — Platform</td>";
        private readonly string SomeHostName = "<td>host</td>";

        [SetUp]
        public void Setup()
        {
            htmlEventsExtractorService = new HtmlEventExtractorService();
        }

        [Test]
        public void Create_Simple_Event()
        {
            // Given
            var expectedStartDateTime = "2021-01-03T10:30:00Z";
            var expectedGameName = "Game";
            var expectedRunnerName = "Runner";
            var expectedSetupLenghtDuration = "0:10:00";
            var expectedEventDuration = "0:20:00";
            var expectedCondition = "Condition";
            var expectedGamePlatform = "Platform";
            var expectedHostName = "host";

            string simpleEvent = string.Concat(new [] {SomeStartDate, SomeGameName, SomeRunnerName,
                SomeSetupDuration, SomeEventDuration, SomeConditionAndPlatform, SomeHostName});

            // When
            var result = htmlEventsExtractorService.CreateRawEvent(simpleEvent);

            // Then
            Assert.AreEqual(expectedStartDateTime, result.StartDateTime);
            Assert.AreEqual(expectedGameName, result.Game.ToString());
            Assert.AreEqual(expectedRunnerName, result.Runners.ToString());
            Assert.AreEqual(expectedSetupLenghtDuration, result.SetupLenght);
            Assert.AreEqual(expectedEventDuration, result.EventDuration);
            Assert.AreEqual(expectedCondition, result.Condition.ToString());
            Assert.AreEqual(expectedGamePlatform, result.GamePlatform.ToString());
            Assert.AreEqual(expectedHostName, result.Host.ToString());
        }

        [Test]
        public void Create_Event_With_Class_In_Div_On_Start_Date()
        {
            // Given
            var expectedStartDateTime = "2021-01-03T10:30:00Z";
            var SomeStartDateWithClass = "<td class='start - time text - right'>2021-01-03T10:30:00Z</td>";

            string simpleEvent = string.Concat(new[] {SomeStartDateWithClass, SomeGameName, SomeRunnerName,
                SomeSetupDuration, SomeEventDuration, SomeConditionAndPlatform, SomeHostName});

            // When
            var result = htmlEventsExtractorService.CreateRawEvent(simpleEvent);

            // Then
            Assert.AreEqual(expectedStartDateTime, result.StartDateTime);
        }

        [Test]
        public void Create_Event_With_Spaces_In_Game_Name()
        {
            // Given
            var expectedGameName = "Just Cause 3";
            var SomeGameWithSpaces = "<td> Just Cause 3 </td>";

            string simpleEvent = string.Concat(new[] {SomeStartDate, SomeGameWithSpaces, SomeRunnerName,
                SomeSetupDuration, SomeEventDuration, SomeConditionAndPlatform, SomeHostName});

            // When
            var result = htmlEventsExtractorService.CreateRawEvent(simpleEvent);

            // Then
            Assert.AreEqual(expectedGameName, result.Game.ToString());
        }

        [Test]
        public void Create_Event_With_Spaces_In_Runner_Name()
        {
            // Given
            var expectedRunnerName = "pmc TRILOG Y";
            var SomeRunnerNameWithSpaces = "<td> pmc TRILOG Y </td>";

            string simpleEvent = string.Concat(new[] {SomeStartDate, SomeGameName, SomeRunnerNameWithSpaces,
                SomeSetupDuration, SomeEventDuration, SomeConditionAndPlatform, SomeHostName});

            // When
            var result = htmlEventsExtractorService.CreateRawEvent(simpleEvent);

            // Then
            Assert.AreEqual(expectedRunnerName, result.Runners.ToString());
        }

        [Test]
        public void Create_Event_With_Two_O_More_Runners_Name()
        {
            // Given
            var expectedRunnerName = "Shockwve, MunchaKoopas, Tokeegee, Traderkirk, Scoobyfoo, BystanderTim";
            var SomeRunnerNameWithSpaces = "<td> Shockwve, MunchaKoopas, Tokeegee, Traderkirk, Scoobyfoo, BystanderTim </td>";

            string simpleEvent = string.Concat(new[] {SomeStartDate, SomeGameName, SomeRunnerNameWithSpaces,
                SomeSetupDuration, SomeEventDuration, SomeConditionAndPlatform, SomeHostName});

            // When
            var result = htmlEventsExtractorService.CreateRawEvent(simpleEvent);

            // Then
            Assert.AreEqual(expectedRunnerName, result.Runners.ToString());
        }

        [Test]
        public void Create_Event_With_Complex_Data_In_Setup_Leght_Duration()
        {
            // Given
            var expectedSetupLenghtDuration = "0:10:00";

            var SomeSetupLeghtDurationWithComplexData =
                "<td rowspan = '2' class='visible-lg text-center'> <i class='fa fa-clock-o text-gdq-red' aria-hidden='true'></i> 0:10:00 </td>";

            string simpleEvent = string.Concat(new[] {SomeStartDate, SomeGameName, SomeRunnerName,
                SomeSetupLeghtDurationWithComplexData, SomeEventDuration, SomeConditionAndPlatform, SomeHostName});

            // When
            var result = htmlEventsExtractorService.CreateRawEvent(simpleEvent);

            // Then
            Assert.AreEqual(expectedSetupLenghtDuration, result.SetupLenght);
        }


        [Test]
        public void Create_Event_Without_Setup_Leght_Duration()
        {
            // Given
            var expectedSetupLenghtDuration = "00:00:00";

            var WithoutSetupLeghtDuration = "<td></td>";

            string simpleEvent = string.Concat(new[] {SomeStartDate, SomeGameName, SomeRunnerName,
                WithoutSetupLeghtDuration, SomeEventDuration, SomeConditionAndPlatform, SomeHostName});

            // When
            var result = htmlEventsExtractorService.CreateRawEvent(simpleEvent);

            // Then
            Assert.AreEqual(expectedSetupLenghtDuration, result.SetupLenght);
        }


        [Test]
        public void Create_Event_With_Complex_Data_In_Event_Duration()
        {
            // Given
            var expectedEventDuration = "0:20:00";
            var SomeEventDurationWithComplexData =
                 "<td class='text-right'> <i class='fa fa-clock-o' aria-hidden='true'></i> 0:20:00 </td>";

            string simpleEvent = string.Concat(new[] {SomeStartDate, SomeGameName, SomeRunnerName,
                SomeSetupDuration, SomeEventDurationWithComplexData, SomeConditionAndPlatform, SomeHostName});

            // When
            var result = htmlEventsExtractorService.CreateRawEvent(simpleEvent);

            // Then
            Assert.AreEqual(expectedEventDuration, result.EventDuration);
        }


        [Test]
        public void Create_Event_With_Spaces_In_Condition_And_Game_Platform()
        {
            // Given
            var expectedCondition = "All Challenges";
            var expectedGamePlatform = "Wii U";

            var SomeConditionAndPlatformWithSpaces = "<td> All Challenges — Wii U </td>";

            string simpleEvent = string.Concat(new[] {SomeStartDate, SomeGameName, SomeRunnerName,
                SomeSetupDuration, SomeEventDuration, SomeConditionAndPlatformWithSpaces, SomeHostName});

            // When
            var result = htmlEventsExtractorService.CreateRawEvent(simpleEvent);

            // Then
            Assert.AreEqual(expectedCondition, result.Condition.ToString());
            Assert.AreEqual(expectedGamePlatform, result.GamePlatform.ToString());
        }

        [Test]
        public void Create_Event_With_Any_Percentage_In_Condition()
        {
            // Given
            var expectedCondition = "Any%";
            var SomeConditionAndPlatformWithSpaces = "<td> Any% — PC </td>";

            string simpleEvent = string.Concat(new[] {SomeStartDate, SomeGameName, SomeRunnerName,
                SomeSetupDuration, SomeEventDuration, SomeConditionAndPlatformWithSpaces, SomeHostName});

            // When
            var result = htmlEventsExtractorService.CreateRawEvent(simpleEvent);

            // Then
            Assert.AreEqual(expectedCondition, result.Condition.ToString());
        }

        [Test]
        public void Create_Event_With_Quates_In_Condition()
        {
            // Given
            var expectedCondition = "Stop 'n' Swop";
            var SomeConditionAndPlatformWithSpaces = "<td> Stop 'n' Swop — N64 </td>";

            string simpleEvent = string.Concat(new[] {SomeStartDate, SomeGameName, SomeRunnerName,
                SomeSetupDuration, SomeEventDuration, SomeConditionAndPlatformWithSpaces, SomeHostName});

            // When
            var result = htmlEventsExtractorService.CreateRawEvent(simpleEvent);

            // Then
            Assert.AreEqual(expectedCondition, result.Condition.ToString());
        }

        [Test]
        public void Create_Event_With_Complex_In_Condition()
        {
            // Given
            var expectedCondition = "Any%(NTSC-1.0)";
            var SomeConditionAndPlatformWithSpaces = "<td> Any%(NTSC-1.0) — N64 </td>";

            string simpleEvent = string.Concat(new[] {SomeStartDate, SomeGameName, SomeRunnerName,
                SomeSetupDuration, SomeEventDuration, SomeConditionAndPlatformWithSpaces, SomeHostName});

            // When
            var result = htmlEventsExtractorService.CreateRawEvent(simpleEvent);

            // Then
            Assert.AreEqual(expectedCondition, result.Condition.ToString());
        }

        [Test]
        public void Create_Event_With_Two_Conditions()
        {
            // Given
            var expectedCondition = "Miriam Any%, MG";
            var SomeConditionAndPlatformWithSpaces = "<td> Miriam Any%, MG — PC </td>";

            string simpleEvent = string.Concat(new[] {SomeStartDate, SomeGameName, SomeRunnerName,
                SomeSetupDuration, SomeEventDuration, SomeConditionAndPlatformWithSpaces, SomeHostName});

            // When
            var result = htmlEventsExtractorService.CreateRawEvent(simpleEvent);

            // Then
            Assert.AreEqual(expectedCondition, result.Condition.ToString());
        }


        [Test]
        public void Throw_Ecdeption_When_Create_Event_With_Invalid_Char_For_Separator()
        {
            // Given
            var SomeConditionAndPlatformWithInvalidSeparator = "<td> All Challenges $ PC </td>";

            string simpleEvent = string.Concat(new[] {SomeStartDate, SomeGameName, SomeRunnerName,
                SomeSetupDuration, SomeEventDuration, SomeConditionAndPlatformWithInvalidSeparator, SomeHostName});

            // When - Then
            Assert.Throws<InvalidNormailizeDataException>(() => htmlEventsExtractorService.CreateRawEvent(simpleEvent));
        }

        [Test]
        public void Create_Event_With_Complex_Host_Name()
        {
            // Given
            var expectedHostName = "host";
            var SomeComplexHostName = "<td><i class='fa fa-microphone'></i> host </td>";

            string simpleEvent = string.Concat(new[] {SomeStartDate, SomeGameName, SomeRunnerName,
                SomeSetupDuration, SomeEventDuration, SomeConditionAndPlatform, SomeComplexHostName});

            // When
            var result = htmlEventsExtractorService.CreateRawEvent(simpleEvent);

            // Then
            Assert.AreEqual(expectedHostName, result.Host.ToString());
        }
    }
}
