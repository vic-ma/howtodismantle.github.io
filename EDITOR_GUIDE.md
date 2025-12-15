# Editor Guide: Homepage Collections & Learning

The home page is uses front‑matter in posts to display articles in collections, learning paths and the featured spot at the top of the home page. If nothing is defined, it will fall back to the newest articles for a few linked categories corresponding with the collection/learning path.

## Key concepts

### Collections
- **What**: 3 curated homepage rows (API & Integrations, Hardware & Devices, Developer Guides).
- **Homepage behavior**:
  - Shows 3 posts max per row.
  - Adds a “View all” tile at the end of each row linking to the collection hub page. This page is also visible in the top navigation bar.
  - A post appears only once across the homepage (hero + rows + latest grid). If it’s used earlier, it won’t be repeated later.
- **Pages**:
  - `/collections/` (overview)
  - `/collections/<collection-id>` (full listing)

Collection definitions live in:
- `_data/collections.yml` (title, description, `tags_fallback`)

Copywriting can be found and adjusted in:
- `_data/ui_text.yml`

### Learning Paths
- **What**: 4 “Learning Paths” cards on the homepage (Hub Flows, Hardware & Sensors, SAP, UI Design).
- **Homepage behavior**:
  - Renders a 4‑card grid (not a post row). Each card has:
    - an icon
    - a title + description
    - a primary CTA button: START HERE
    - 3 secondary links (the small bullet links under the button)
  - User can navigate via each card’s START HERE button.
- **Pages**:
  - `/learning/` (overview)
  - `/learning/<learning-id>` (full flow listing)

Learning definitions live in:
- `_data/learning.yml`

Copywriting can be found and adjusted in:
- `_data/ui_text.yml`

## How curation works

### 1) Featured hero post (homepage)
To manually pick the hero post:

```yml
home_featured: true
home_featured_order: 10
```

- Lower `home_featured_order` wins.
- If no post has `home_featured: true`, the hero falls back to the latest post.

### 2) Curate posts into Collections (with order)
Add a `collections` map to a post’s front‑matter:

```yml
collections:
  api-integrations: 10
  hardware-devices: 30
```

- The number is the order (lower shows first).
- If you don’t curate anything for a collection yet, the collection row/hub page falls back to newest posts that match the tags listed in `_data/collections.yml` under `tags_fallback`.

Collection IDs:
- `api-integrations`
- `hardware-devices`
- `developer-guides`

### 3) Curate posts into Learning (with order)
Add a `learning` map to a post’s front‑matter:

```yml
learning:
  sap: 10
  ui-design: 20
```

- The number is the step order (lower shows first).
- If you don’t curate anything for a learning flow yet, it falls back to newest posts that match the `tags_fallback` in `_data/learning.yml`.

Learning IDs:
- `hub-flows`
- `hardware-sensors`
- `sap`
- `ui-design`

## Examples (copy/paste)

### Example: Make a post the homepage hero AND first in API collection

```yml
home_featured: true
home_featured_order: 10

collections:
  api-integrations: 10
```

### Example: Add a post to a learning flow as “Step 2”

```yml
learning:
  hardware-sensors: 20
```

## Notes / gotchas
- **Ordering is numeric**. Use 10, 20, 30… so you can insert steps later.
- **Single placement** on the homepage means: if a post is featured in the hero/rows, it won’t show again in later sections.
- If you want a post to appear in a hub page but not on the homepage row, just keep the `collections`/`learning` key and adjust homepage curation later (we can add “exclude_home: true” logic if needed).


