from providers import data_provider
import unittest


class TestOrders(unittest.TestCase):
    def setUp(self) -> None:
        self.orders = data_provider.Orders("../data/")

    def test_loaded(self):
        self.assertGreater(len(self.orders.get_orders()), 0)


if __name__ == "__main__":
    unittest.main()