#set heading(numbering: "1.1")
#set block(breakable: false)

= List of Blocks
#for (block, attributes) in (
  toml("block_data.toml").pairs().sorted(key: it => it.at(1)._id)
) {
  [== #block
    #table(
      columns: 2,
      ..attributes
        .pairs()
        .sorted()
        .map(p => (
          [*#p.at(0).replace(regex("^_"), "").replace("_", " ")*],
          [#p.at(1)],
        ))
        .flatten()
    )
  ]
}
