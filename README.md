<p align="center">
    <img src="assets/icon.png" margin="auto" height="175"/>
</p>
<p align="center">
    <a href="https://github.com/bmresearch/Solnet.Mango/actions/workflows/dotnet.yml">
        <img src="https://github.com/bmresearch/Solnet.Mango/actions/workflows/dotnet.yml/badge.svg"
            alt="Build Status (master)" ></a>
    <a href="https://coveralls.io/github/bmresearch/Solnet.Mango?branch=master">
        <img src="https://coveralls.io/repos/github/bmresearch/Solnet.Mango/badge.svg?branch=master" 
            alt="Coverage Status" ></a>
</p>

# What is Solnet.Mango?

[Solnet](https://github.com/bmresearch/Solnet) is Solana's .NET integration library, a number of packages that implement features to interact with
Solana from .Net applications.

Solnet.Mango is a package within the same `Solnet.` namespace that implements a Client for [Mango Markets](https://mango.markets/), this project is in a
separate repository so it is contained, as the goal for [Solnet](https://github.com/bmresearch/Solnet) was to be a core SDK.

## Features

- Decoding of Mango data structures:
    - `MangoGroup`
    - `MangoCache`
    - `MangoAccount`
    - `AdvancedOrdersAccount`
    - `PerpMarket`
    - `RootBank`
    - `NodeBank`
    - `OrderBookSide` (`Node`s are decoded and processed into a friendlier structure)
    - `EventQueue`
- `MangoProgram` instructions implemented:
  - InitMangoAccount
  - Deposit
  - Withdraw
  - PlaceSpotOrder
  - PlaceSpotOrder2
  - PlacePerpOrder
  - CancelPerpOrderByClientId
  - CancelPerpOrder
  - SettleFunds
  - CancelSpotOrder
  - SettleProfitAndLoss
  - SettleFees
  - InitSpotOpenOrders
  - RedeemMango
  - AddMangoAccountInfo
  - CancelAllPerpOrders
  - InitAdvancedOrders
  - AddPerpTriggerOrder
  - RemoveAdvancedOrder
  - ExecutePerpTriggerOrder
  - CloseMangoAccount
  - CloseSpotOpenOrders
  - CloseAdvancedOrders
  - CreateMangoAccount
  - UpgradeMangoAccountV0V1
  - CancelPerpOrdersSide
  - SetDelegate
- `MangoClient` class which allows to:
    - Get these structures and decode them only by having their address
    - Subscribing to these accounts in real time, getting notifications with their decoded structures

## Requirements
- net 5.0

## Dependencies
- Solnet.Serum v1.2.0
- Solnet.Wallet v0.5.0
- Solnet.Rpc v0.5.0

## Examples

The [Solnet.Mango.Examples](https://github.com/bmresearch/Solnet.Mango/tree/master/Solnet.Mango.Examples) project features some examples on how to use the [IMangoClient](https://github.com/bmresearch/Solnet.Mango/tree/master/Solnet.Mango/IMangoClient.cs), these examples include:
- Getting all mango accounts owned by a specific address
- Streaming market data and various structures
- Submitting orders (spot, perps)
- Cancelling orders (spot, perps)
- Creating new mango accounts

## Contribution

We encourage everyone to contribute, submit issues, PRs, discuss. Every kind of help is welcome.

## Contributors

* **Hugo** - *Maintainer* - [murlokito](https://github.com/murlokito)
* **Tiago** - *Maintainer* - [tiago](https://github.com/tiago18c)

See also the list of [contributors](https://github.com/bmresearch/Solnet.Serum/contributors) who participated in this project.

## License

This project is licensed under the MIT License - see the [LICENSE](https://github.com/bmresearch/Solnet.Serum/blob/master/LICENSE) file for details
