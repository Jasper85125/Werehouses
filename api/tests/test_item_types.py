import unittest
from unittest.mock import patch, mock_open
from models.item_types import ItemTypes

class TestItemTypes(unittest.TestCase):
    def setUp(self) -> None:
        self.mock_data = [
            {"id": 0, "name": "Laptop", "description": "", "created_at": "2001-11-02 23:02:40", "updated_at": "2008-07-01 04:09:17"},
            {"id": 1, "name": "Desktop", "description": "", "created_at": "1993-07-28 13:43:32", "updated_at": "2022-05-12 08:54:35"}
        ]
        self.itemTypes = ItemTypes("../data/", is_debug=True)
        self.itemTypes.data = self.mock_data

    def test_loaded(self):
        self.assertGreater(len(self.itemTypes.get_item_types()), 0)

    def test_add_item_type(self):
        new_item = {"id": 2, "name": "Tablet", "description": ""}
        self.itemTypes.add_item_type(new_item)
        self.assertEqual(len(self.itemTypes.get_item_types()), 3)
        self.assertIsNotNone(self.itemTypes.get_item_type(2))

    def test_update_item_type(self):
        updated_item = {"id": 0, "name": "Gaming Laptop", "description": ""}
        self.itemTypes.update_item_type(0, updated_item)
        self.assertEqual(self.itemTypes.get_item_type(0)["name"], "Gaming Laptop")

    def test_remove_item_type(self):
        self.itemTypes.remove_item_type(1)
        self.assertIsNone(self.itemTypes.get_item_type(1))
        self.assertEqual(len(self.itemTypes.get_item_types()), 1)

    @patch("builtins.open", new_callable=mock_open, read_data='[{"id": 0, "name": "Laptop"}]')
    def test_load(self, mock_file):
        self.itemTypes.load(is_debug=False)
        self.assertEqual(len(self.itemTypes.get_item_types()), 1)
        self.assertEqual(self.itemTypes.get_item_type(0)["name"], "Laptop")

    @patch("builtins.open", new_callable=mock_open)
    def test_save(self, mock_file):
        self.itemTypes.save()
        handle = mock_file()
        written_data = ''.join(call.args[0] for call in handle.write.call_args_list)
        expected_data = '[{"id": 0, "name": "Laptop", "description": "", "created_at": "2001-11-02 23:02:40", "updated_at": "2008-07-01 04:09:17"}, {"id": 1, "name": "Desktop", "description": "", "created_at": "1993-07-28 13:43:32", "updated_at": "2022-05-12 08:54:35"}]'
        self.assertEqual(written_data, expected_data)

if __name__ == "__main__":
    unittest.main()