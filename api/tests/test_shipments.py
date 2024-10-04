from providers import data_provider
import unittest


class TestShipments(unittest.TestCase):
    def setUp(self) -> None:
        self.shipments = data_provider.Shipments("../data/")

    def test_loaded(self):
        self.assertGreater(len(self.shipments.get_shipments()), 0)


if __name__ == "__main__":
    unittest.main()