from models.clients import Clients
import unittest


class TestClients(unittest.TestCase):
    def setUp(self) -> None:
        self.clients = Clients("../data/")

    def test_loaded(self):
        self.assertGreater(len(self.clients.get_clients()), 0)


if __name__ == "__main__":
    unittest.main()