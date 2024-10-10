from models.clients import Clients
import unittest


class TestClients(unittest.TestCase):
    def setUp(self) -> None:
        self.clients = Clients("../data/")

    def test_loaded(self):
        self.assertGreater(len(self.clients.get_clients()), 0)

    def test_get_client_by_id(self):
        # self.assertEqual()
        self.assertIsNotNone(self.clients.get_client(1))
    
    def test_add_client(self):
        numClients = len(self.clients.get_clients())
        new_client = {
            "id": numClients + 1,
            "name": "Test Client",
            "created_at": self.clients.get_timestamp()
        }
        self.clients.add_client(new_client)
        self.assertGreater(len(self.clients.get_clients()), numClients)

    def test_update_client(self):
        current = self.clients.get_client(client_id=1)
        updated_client = {
            "id": current["id"],
            "name": "Updated Client",
            "created_at": current["created_at"],
            "updated_at": self.clients.get_timestamp()
        }
        self.clients.update_client(1, updated_client)
        updated = self.clients.get_client(client_id=1)
        self.assertNotEqual(current, updated)
        self.assertEqual(updated["name"], "Updated Client")

    def test_remove_client(self):
        numClients = self.clients.get_clients()
        self.clients.remove_client(1)
        self.assertNotEqual(len(self.clients.get_clients()), numClients)


if __name__ == "__main__":
    unittest.main()
