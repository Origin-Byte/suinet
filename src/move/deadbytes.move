module nft_protocol::deadbytes {
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
    use nft_protocol::mint_cap::MintCap;
    use nft_protocol::warehouse::{Self, Warehouse};
    use nft_protocol::royalties::{Self, TradePayment};
    use nft_protocol::collection::{Self, Collection};
    use nft_protocol::witness;

    /// One time witness is only instantiated in the init method
    struct DEADBYTES has drop {}

    /// Can be used for authorization of other actions post-creation. It is
    /// vital that this struct is not freely given to any contract, because it
    /// serves as an auth token.
    struct Witness has drop {}

    fun init(witness: DEADBYTES, ctx: &mut TxContext) {
        let (mint_cap, collection) = collection::create(
            &witness, ctx,
        );
        let delegated_witness = witness::from_witness(&Witness {});
        collection::add_domain(
            delegated_witness,
            &mut collection,
            creators::from_address<DEADBYTES, Witness>(&Witness {}, tx_context::sender(ctx)            )
        );

        // Register custom domains
        display::add_collection_display_domain(
            delegated_witness,
            &mut collection,
            string::utf8(b"DeadBytes"),
            string::utf8(b"A unique NFT collection of DeadBytes on Sui")
        );

        display::add_collection_url_domain(
            delegated_witness,
            &mut collection,
            sui::url::new_unsafe_from_bytes(b"https://originbyte.io/")
        );

        display::add_collection_symbol_domain(
            delegated_witness,
            &mut collection,
            string::utf8(b"SUIM")
        );

        let royalty = royalty::from_address(tx_context::sender(ctx), ctx);
        royalty::add_proportional_royalty(&mut royalty, 100);

        royalty::add_royalty_domain(delegated_witness, &mut collection, royalty);

        let tags = tags::empty(ctx);
        tags::add_tag(&mut tags, tags::art());
        tags::add_collection_tag_domain(delegated_witness, &mut collection, tags);

        transfer::transfer(mint_cap, tx_context::sender(ctx));
        transfer::share_object(collection);
    }

    /// Calculates and transfers royalties to the `RoyaltyDomain`
    public entry fun collect_royalty<FT>(
        payment: &mut TradePayment<DEADBYTES, FT>,
        collection: &mut Collection<DEADBYTES>,
        ctx: &mut TxContext,
    ) {
        let b = royalties::balance_mut(Witness {}, payment);

        let domain = royalty::royalty_domain(collection);
        let royalty_owed =
            royalty::calculate_proportional_royalty(domain, balance::value(b));

        royalty::collect_royalty(collection, b, royalty_owed);
        royalties::transfer_remaining_to_beneficiary(Witness {}, payment, ctx);
    }

    // public entry fun mint_nft(
    //     name: String,
    //     description: String,
    //     url: vector<u8>,
    //     attribute_keys: vector<String>,
    //     attribute_values: vector<String>,
    //     _mint_cap: &MintCap<DEADBYTES>,
    //     warehouse: &mut Warehouse,
    //     ctx: &mut TxContext,
    // ) {
    //     let nft = nft::new<DEADBYTES, Witness>(
    //         &Witness {}, tx_context::sender(ctx), ctx
    //     );

    //     display::add_display_domain(
    //         &mut nft,
    //         name,
    //         description,
    //         ctx,
    //     );

    //     display::add_url_domain(
    //         &mut nft,
    //         url::new_unsafe_from_bytes(url),
    //         ctx,
    //     );

    //     display::add_attributes_domain_from_vec(
    //         &mut nft,
    //         attribute_keys,
    //         attribute_values,
    //         ctx,
    //     );

    //     warehouse::deposit_nft(warehouse, nft);
    // }

    public entry fun airdrop(
        name: String,
        description: String,
        url: vector<u8>,
        attribute_keys: vector<String>,
        attribute_values: vector<String>,
        _mint_cap: &MintCap<DEADBYTES>,
        receiver: address,
        ctx: &mut TxContext,
    ) {
        let nft = nft::new<DEADBYTES, Witness>(
            &Witness {}, name, url::new_unsafe_from_bytes(url), tx_context::sender(ctx), ctx
        );
        let delegated_witness = witness::from_witness(&Witness {});

        display::add_display_domain(
            delegated_witness,
            &mut nft,
            name,
            description
        );

        display::add_url_domain(
            delegated_witness,
            &mut nft,
            url::new_unsafe_from_bytes(url)
        );
        display::add_attributes_domain_from_vec(
            delegated_witness,
            &mut nft,
            attribute_keys,
            attribute_values
        );

        transfer::transfer(nft, receiver);
    }
}