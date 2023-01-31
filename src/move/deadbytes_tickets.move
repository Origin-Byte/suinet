module nft_protocol::deadbytes_tickets {
    use std::string::{Self, String};

    use sui::url;
    use sui::balance;
    use sui::transfer;
    use sui::tx_context::{Self, TxContext};

    use nft_protocol::nft;
    use nft_protocol::tags;
    use nft_protocol::royalty;
    use nft_protocol::display;
    use nft_protocol::creators;
    use nft_protocol::inventory::{Self, Inventory};
    use nft_protocol::royalties::{Self, TradePayment};
    use nft_protocol::collection::{Self, Collection, MintCap};
    use nft_protocol::gaming;

    /// One time witness is only instantiated in the init method
    struct DEADBYTES_TICKETS has drop {}

    /// Can be used for authorization of other actions post-creation. It is
    /// vital that this struct is not freely given to any contract, because it
    /// serves as an auth token.
    struct Witness has drop {}

    fun init(witness: DEADBYTES_TICKETS, ctx: &mut TxContext) {
        let (mint_cap, collection) = collection::create(
            &witness, ctx,
        );

        collection::add_domain(
            &mut collection,
            &mut mint_cap,
            creators::from_address(tx_context::sender(ctx))
        );

        // Register custom domains
        display::add_collection_match_invite_domain(
            &mut collection,
            &mut mint_cap,
            string::utf8(b"DummyMatchId"),
        );

        transfer::transfer(mint_cap, tx_context::sender(ctx));
        transfer::share_object(collection);
    }

    public entry fun mint_nft(
        matchId: String,
        attribute_keys: vector<String>,
        attribute_values: vector<String>,
        _mint_cap: &MintCap<DEADBYTES>,
        inventory: &mut Inventory,
        ctx: &mut TxContext,
    ) {
        let nft = nft::new<DEADBYTES, Witness>(
            &Witness {}, tx_context::sender(ctx), ctx
        );

        display::add_match_invite_domain(
            &mut nft,
            matchId,
            ctx,
        );

        inventory::deposit_nft(inventory, nft);
    }

    public entry fun airdrop(
        matchId: String,
        _mint_cap: &MintCap<DEADBYTES>,
        receiver: address,
        ctx: &mut TxContext,
    ) {
        let nft = nft::new<DEADBYTES, Witness>(
            &Witness {}, tx_context::sender(ctx), ctx
        );

        display::add_match_invite_domain(
            &mut nft,
            matchId,
            ctx,
        );

        transfer::transfer(nft, receiver);
    }
}