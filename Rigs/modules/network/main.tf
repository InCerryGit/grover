variable "address_space" {
    type = string
    default = "10.0.0.0/16"
}

variable "default_subnet_cidr" {
    type = string 
    default = "10.0.2.0/24"
}

variable "location" {
    type = string
}

resource "azurerm_resource_group" "basic_rig_network_rg" {
    name = "Grover-Network"
    location = var.location
}

resource "azurerm_virtual_network" "basic_rig_vnet" {
    name                = "basic-vnet"
    address_space       = [var.address_space]
    location            = azurerm_resource_group.basic_rig_network_rg.location
    resource_group_name = azurerm_resource_group.basic_rig_network_rg.name
}

resource "azurerm_subnet" "basic_rig_subnet" {
 name                 = "basic-vnet-subnet"
 resource_group_name  = azurerm_resource_group.basic_rig_network_rg.name
 virtual_network_name = azurerm_virtual_network.basic_rig_vnet.name
 address_prefix       = var.default_subnet_cidr
}

output "name" {
    value = "BackendNetwork"
}

output "subnet_instance_id" {
    value = azurerm_subnet.basic_rig_subnet.id
}

output "networkrg_name" {
    value = azurerm_resource_group.basic_rig_network_rg.name
} 