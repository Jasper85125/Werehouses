# import json
import unittest
import httpx
# from models.base import Base

# ITEM_LINES = []


# class ItemLines(Base):
#     def __init__(self, root_path, is_debug=False):
#         self.data_path = root_path + "item_lines.json"
#         self.load(is_debug)

#     def get_item_lines(self):
#         return self.data

#     def get_item_line(self, item_line_id):
#         for x in self.data:
#             if x["id"] == item_line_id:
#                 return x
#         return None

#     def add_item_line(self, item_line):
#         item_line["created_at"] = self.get_timestamp()
#         item_line["updated_at"] = self.get_timestamp()
#         self.data.append(item_line)

#     def update_item_line(self, item_line_id, item_line):
#         item_line["updated_at"] = self.get_timestamp()
#         for i in range(len(self.data)):
#             if self.data[i]["id"] == item_line_id:
#                 self.data[i] = item_line
#                 break

#     def remove_item_line(self, item_line_id):
#         self.data = [x for x in self.data if x["id"] != item_line_id]

#     def load(self, is_debug):
#         if is_debug:
#             self.data = ITEM_LINES
#         else:
#             f = open(self.data_path, "r")
#             self.data = json.load(f)
#             f.close()

#     def save(self):
#         f = open(self.data_path, "w")
#         json.dump(self.data, f)
#         f.close()


# if __name__ == "__main__":
#     unittest.main()

def checkItemLine(item_line):
    required_properties = ["id", "name", "description",
                           "created_at", "updated_at"]
    for prop in required_properties:
        if item_line.get(prop) is None:
            return False
    return True


class TestItemLines(unittest.TestCase):
    def setUp(self):
        self.client = httpx.Client()
        self.url = "http://localhost:3000/api/v1"
        self.headers = httpx.Headers({'API_KEY': 'a1b2c3d4e5'})

    def test_get_item_line_id(self):
        # Send the request
        response = self.client.get(
            url=(self.url + "/item_lines/0"), headers=self.headers
        )
        # Check the status code
        self.assertEqual(response.status_code, 200)

        # Check that the response is a dictionary
        # (representative of a single item line object)
        self.assertEqual(type(response.json()), dict)

        # Check that the item line object has the correct properties
        self.assertTrue(checkItemLine(response.json()))

    def test_get_item_lines(self):
        # Send the request
        response = self.client.get(
            url=(self.url + "/item_lines"),
            headers=self.headers
        )

        # Check the status code
        self.assertEqual(response.status_code, 200)

        # Check that the response is a list
        # (representative of a list of item lines)
        self.assertEqual(type(response.json()), list)

        # If the list contains something, check the first object in the list
        if len(response.json()) > 0:
            # Check that each object in the list is a dictionary
            self.assertEqual(type(response.json()[0]), dict)

            # Check that each item line object has the correct properties
            self.assertTrue(
                all(
                    checkItemLine(item_line)
                    for item_line in response.json()
                )
            )

    # Overwrites an item line based on the given item line id
    def test_put_item_line_id(self):
        data = {
            "id": 0,
            "name": "Updated Item Line",
            "description": "Updated description",
            "created_at": "2022-08-18 07:05:25",
            "updated_at": "2023-10-01 12:00:00"
        }

        # Send the request
        response = self.client.put(
            url=(self.url + "/item_lines/0"),
            headers=self.headers,
            json=data
        )

        # Check the status code
        self.assertEqual(response.status_code, 200)

    # This deletes an item line based on an id
    def test_delete_item_line_id(self):
        # Send the request
        response = self.client.delete(
            url=(self.url + "/item_lines/100"), headers=self.headers
        )

        # Check the status code
        self.assertEqual(response.status_code, 200)


# to run the file: python -m unittest test_item_lines.py
# # git checkout . -f
