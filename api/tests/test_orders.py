import unittest
import requests

def checkOrder(order):
    required_properties = ["source_id", "order_date", "request_date", "order_status",
                           "warehouse_id", "shipment_id", "total_amount", "total_discount",
                           "total_tax", "total_surcharge", "items"]
    for prop in required_properties:
        if order.get(prop) is None:
            return False
    return True

class TestOrdersAPI(unittest.TestCase):
    def setUp(self):
        self.url = "http://localhost:3000/api/v1"
        self.headers = {'API_KEY': 'a1b2c3d4e5'}

    def test_get_orders(self):
        response = requests.get(f"{self.url}/orders", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        self.assertEqual(type(response.json()), list)
        
        if len(response.json()) > 0:
            self.assertTrue(checkOrder(order)for order in response.json()[0])

    def test_get_order_by_id(self):
        response = requests.get(f"{self.url}/orders/1", headers=self.headers)
        self.assertEqual(response.status_code, 200)
        self.assertEqual(type(response.json()), dict)

        if len(response.json()) == 1:
            self.assertTrue(checkOrder(order)for order in response.json())
    
    #c# fix
    def test_get_order_by_non_existing_id(self):
        response = requests.get(f"{self.url}/orders/10000000", headers=self.headers)
        self.assertEqual(response.status_code, 404)
        self.assertEqual(type(response.json()), type(None))

    def test_get_wrong_path(self):
        response = requests.get(url=(self.url + "/orders/1/error"), headers=self.headers)

        self.assertEqual(response.status_code, 404)

    def test_post_order(self):
        data = {
        "id": 1,
        "source_id": 33,
        "order_date": "2019-04-03T11:33:15Z",
        "request_date": "2019-04-07T11:33:15Z",
        "reference": None,
        "reference_extra": None,
        "order_status": "Delivered",
        "notes": None,
        "shipping_notes": None,
        "picking_notes": None,
        "warehouse_id": 18,
        "ship_to": None,
        "bill_to": None,
        "shipment_id": 1,
        "total_amount": 9905.13,
        "total_discount": 150.77,
        "total_tax": 372.72,
        "total_surcharge": 77.6,
        "created_at": None,
        "updated_at": None,
        "items": [{
                    "item_id": "P007435",
                    "amount": 23
                  },
                  {
                    "item_id": "P009557",
                    "amount": 1
                }]}
        
        response = requests.post(f"{self.url}/orders", headers=self.headers, json=data)

        self.assertTrue(checkOrder(data))
        self.assertEqual(response.status_code, 201)
    
    #C# fix
    def test_post_order_wrong_info(self):
        data = {
        "id": 1,
        "source_id": None,
        "order_date": "2019-04-03T11:33:15Z",
        "request_date": "2019-04-07T11:33:15Z",
        "reference": None,
        "reference_extra": None,
        "order_status": "Delivered",
        "notes": None,
        "shipping_notes": None,
        "picking_notes": None,
        "warehouse_id": 18,
        "ship_to": None,
        "bill_to": None,
        "shipment_id": 1,
        "total_amount": 9905.13,
        "total_discount": 150.77,
        "total_tax": 372.72,
        "total_surcharge": 77.6,
        "created_at": None,
        "updated_at": None,
        "items": [{
                    "item_id": "P007435",
                    "amount": 23
                  },
                  {
                    "item_id": "P009557",
                    "amount": 1
                }]}
        
        response = requests.post(f"{self.url}/orders", headers=self.headers, json=data)

        self.assertFalse(checkOrder(data))
        self.assertEqual(response.status_code, 400)

    def test_put_order(self):
        data = {
        "id": 1,
        "source_id": 33,
        "order_date": "2019-04-03T11:33:15Z",
        "request_date": "2019-04-07T11:33:15Z",
        "reference": None,
        "reference_extra": None,
        "order_status": "Delivered",
        "notes": None,
        "shipping_notes": None,
        "picking_notes": None,
        "warehouse_id": 18,
        "ship_to": None,
        "bill_to": None,
        "shipment_id": 1,
        "total_amount": 9905.13,
        "total_discount": 150.77,
        "total_tax": 372.72,
        "total_surcharge": 77.6,
        "created_at": None,
        "updated_at": None,
        "items": [{
                    "item_id": "P007435",
                    "amount": 23
                  },
                  {
                    "item_id": "P009557",
                    "amount": 1
                }]}
        
        response = requests.put(f"{self.url}/orders/1", headers=self.headers, json=data)
        
        self.assertTrue(checkOrder(data))
        self.assertEqual(response.status_code, 200)
    
    #C# fix
    def test_put_order_wrong_info(self):
        data = {
        "id": 1,
        "source_id": 33,
        "order_date": None,
        "request_date": None,
        "reference": None,
        "reference_extra": None,
        "order_status": "Delivered",
        "notes": None,
        "shipping_notes": None,
        "picking_notes": None,
        "warehouse_id": None,
        "ship_to": None,
        "bill_to": None,
        "shipment_id": 1,
        "total_amount": 9905.13,
        "total_discount": 150.77,
        "total_tax": 372.72,
        "total_surcharge": 77.6,
        "created_at": None,
        "updated_at": None,
        "items": [{
                    "item_id": "P007435",
                    "amount": 23
                  },
                  {
                    "item_id": "P009557",
                    "amount": 1
                }]}
        
        response = requests.put(f"{self.url}/orders/1", headers=self.headers, json=data)
        
        self.assertFalse(checkOrder(data))
        self.assertEqual(response.status_code, 400)
    
    #Werkt op dit moment nog niet. Snap niet hoe deze werkt!
    def test_put_items_in_order(self):
        data = {
            "items": [
                {"item_id": "P003790", "amount": 12},
                {"item_id": "P007369", "amount": 18}
            ]
        }
        response = requests.put(f"{self.url}/orders/1/items", headers=self.headers, json=data)
        self.assertEqual(response.status_code, 200)
             
    def test_delete_order(self):
        response = requests.delete(f"{self.url}/orders/1", headers=self.headers)
        self.assertEqual(response.status_code, 200)

    def test_get_items_in_order(self):
        response = requests.get(f"{self.url}/orders/1/items", headers=self.headers)
        self.assertEqual(response.status_code, 200)

    def test_get_orders_for_client(self):
        response = requests.get(f"{self.url}/clients/1/orders", headers=self.headers)
        self.assertEqual(response.status_code, 200)

    def test_get_orders_in_shipment(self):
        response = requests.get(f"{self.url}/shipments/1/orders", headers=self.headers)
        self.assertEqual(response.status_code, 200)


if __name__ == "__main__":
    unittest.main()
