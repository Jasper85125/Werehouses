from models.transfers import Transfers
import unittest


class TestTransfers(unittest.TestCase):
    def setUp(self) -> None:
        self.transfers = Transfers("../data/")

    def test_loaded(self):
        self.assertGreater(len(self.transfers.get_transfers()), 0)


if __name__ == "__main__":
    unittest.main()