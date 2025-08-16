
using System;
using System.Collections.Generic;
using CMSuniVortex.Excel;
using NUnit.Framework;
using Debug = UnityEngine.Debug;

namespace CMSuniVortex.Tests
{
    public sealed class ExcelTest
    {
        const string _packagePath = "Packages/com.ishix.cmsunivortex/";
        const string _xlsxFilePath = _packagePath + "Tests/Editor/GoogleSheetModels-Custom.xlsx";
        const string _xlsbFilePath = _packagePath + "Tests/Editor/GoogleSheetModels-Custom.xlsb";
        const string _xlsFilePath = _packagePath + "Tests/Editor/GoogleSheetModels-Custom.xls";
        
        [TestCase(_xlsxFilePath)]
        [TestCase(_xlsbFilePath)]
        [TestCase(_xlsFilePath)]
        public void SheetLoadTest(string path)
        {
            var sheet = ExcelService.GetSheet(path, "English");
            Assert.That(sheet.TableName, Is.EqualTo("English"));
            Assert.That(sheet.Rows[0].ItemArray[0], Is.EqualTo("Key"));
        }
        
        [TestCase(_xlsxFilePath)]
        [TestCase(_xlsbFilePath)]
        [TestCase(_xlsFilePath)]
        public void SheetPrintAllTest(string path)
        {
            var sheet = ExcelService.GetSheet(path, "English");
            var msg = string.Empty;
            for (var row = 0; row < sheet.Rows.Count; row++) {
                for (var col = 0; col < sheet.Columns.Count; col++) {
                    var cellValue = sheet.Rows[row][col];
                    msg += ("row: " + row + " col: " + col + " | " + cellValue + "\n");
                }
            }
            Debug.Log(msg);
        }
        
        [TestCase(_xlsxFilePath)]
        [TestCase(_xlsbFilePath)]
        [TestCase(_xlsFilePath)]
        public void PrintModifiedTime(string path)
        {
            var time = ExcelService.GetModifiedTime(path);
            var sheetTime = DateTime.Parse(time!.ToString());
            var msg = $"Sheet: {sheetTime:MM/dd/yyyy HH:mm}";
            Debug.Log(msg);
        }
        
        [TestCase(_xlsxFilePath)]
        [TestCase(_xlsbFilePath)]
        [TestCase(_xlsFilePath)]
        public void CheckForUpdateTest(string path)
        {
            var time = ExcelService.GetModifiedTime(path);
            Assert.That(time, Is.Not.Null);
            var nullableDateTime = time?.AddMinutes(1).DateTime;
            var result = ExcelService.CheckForUpdate(path, nullableDateTime);
            var hasUpdate = result.HasUpdate;
            Assert.That(hasUpdate, Is.True);
            var text = hasUpdate
                ? "Updates available. Please import.\n"
                : "You have the latest version.\n";
            text += result.Details;
            Debug.Log(text);
        }
        
        class TestModel : ExcelModel
        {
            IReadOnlyDictionary<string, TestEnum> _maps;
            
            public enum TestEnum { Enum1, Enum2 }
            
            public void SetMap(IReadOnlyDictionary<string, TestEnum> maps) => _maps = maps;
            
            protected override void OnDeserialize()
            {
                {
                    var result = TryGetEnum<TestEnum>("A", _maps, out var value);
                    Assert.That(result, Is.EqualTo(CuvEnumResult.Success));
                    Assert.That(value, Is.EqualTo(TestEnum.Enum1));
                }
                {
                    var result = TryGetEnum<TestEnum>("B", _maps, out var value);
                    Assert.That(result, Is.EqualTo(CuvEnumResult.EmptyValue));
                    Assert.That(value, Is.EqualTo(TestEnum.Enum1));
                }
                {
                    var result = TryGetEnum<TestEnum>("C", _maps, out var value);
                    Assert.That(result, Is.EqualTo(CuvEnumResult.NotFoundInMap));
                    Assert.That(value, Is.EqualTo(TestEnum.Enum1));
                }
                {
                    var result = TryGetEnum<TestEnum>("Z", _maps, out var value);
                    Assert.That(result, Is.EqualTo(CuvEnumResult.NotHasKey));
                    Assert.That(value, Is.EqualTo(TestEnum.Enum1));
                }
            }
        }
        
        [Test]
        public void EnumDeserializeTest()
        {
            var model = new TestModel();
            model.SetMap(new Dictionary<string, TestModel.TestEnum>()
            {
                {"AA", TestModel.TestEnum.Enum1},
                {"BB", TestModel.TestEnum.Enum2},
            });
            
            ((IObjectDeserializer) model).Deserialize(new Dictionary<string, string>()
            {
                {"A", "AA"},
                {"B", ""},
                {"C", "CC"},
            });
        }
    }
}