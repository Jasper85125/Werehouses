import json

from models.base import Base
from providers import data_provider

SHIPMENTS = []


class Shipments(Base):
    def __init__(self, root_path, is_debug=False):
        self.data_path = root_path + "shipments.json"
        self.load(is_debug)

    def get_shipments(self):
        return self.data

    def get_shipment(self, shipment_id):
        for x in self.data:
            if x["id"] == shipment_id:
                return x
        return None

    def get_items_in_shipment(self, shipment_id):
        for x in self.data:
            if x["id"] == shipment_id:
                return x["items"]
        return None

    def add_shipment(self, shipment):
        shipment["created_at"] = self.get_timestamp()
        shipment["updated_at"] = self.get_timestamp()
        self.data.append(shipment)

    def update_shipment(self, shipment_id, shipment):
        shipment["updated_at"] = self.get_timestamp()
        for i in range(len(self.data)):
            if self.data[i]["id"] == shipment_id:
                self.data[i] = shipment
                break

    def update_items_in_shipment(self, shipment_id, items):
        shipment = self.get_shipment(shipment_id)
        current_items = shipment.get("items", [])  # Get current items in the shipment
        new_items = items.get("items", [])         # Get new items from the request data

        # Process items that are removed from the shipment (in current_items but not in new_items)
        for current_item in current_items:
            item_id = current_item.get("item_id")
            matching_new_item = next((ni for ni in new_items if ni.get("item_id") == item_id), None)

            if not matching_new_item:
                # If current_item is not found in new_items, adjust inventory
                inventories = data_provider.fetch_inventory_pool().get_inventories_for_item(item_id)
                for inventory in inventories:
                    inventory["total_ordered"] -= current_item["amount"]
                    inventory["total_expected"] = (
                        inventory.get("total_on_hand", 0) + inventory["total_ordered"]
                    )
                    data_provider.fetch_inventory_pool().update_inventory(inventory["id"], inventory)

        # Process items that are either updated or added in the new items list
        for new_item in new_items:
            item_id = new_item.get("item_id")
            matching_current_item = next((ci for ci in current_items if ci.get("item_id") == item_id), None)

            if matching_current_item:
                # If item exists in current_items, adjust for quantity change
                amount_diff = new_item["amount"] - matching_current_item["amount"]
            else:
                # If item does not exist in current_items, treat the full amount as new
                amount_diff = new_item["amount"]

            inventories = data_provider.fetch_inventory_pool().get_inventories_for_item(item_id)
            for inventory in inventories:
                inventory["total_ordered"] += amount_diff
                inventory["total_expected"] = (
                    inventory.get("total_on_hand", 0) + inventory["total_ordered"]
                )
                data_provider.fetch_inventory_pool().update_inventory(inventory["id"], inventory)

        # Update shipment items with the new items list only
        shipment["items"] = new_items
        self.update_shipment(shipment_id, shipment)

    def remove_shipment(self, shipment_id):
        for x in self.data:
            if x["id"] == shipment_id:
                self.data.remove(x)

    def load(self, is_debug):
        if is_debug:
            self.data = SHIPMENTS
        else:
            f = open(self.data_path, "r")
            self.data = json.load(f)
            f.close()

    def save(self):
        f = open(self.data_path, "w")
        json.dump(self.data, f)
        f.close()
