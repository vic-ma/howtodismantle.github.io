# Editor Guide: Homepage Collections & Learning

The homepage uses post front‑matter to display articles in collections, learning paths, and the featured hero. If nothing is defined, it falls back to newest posts matching tag fallbacks.

## Quick Reference

**Curation Overview:** `/admin/curation-overview/` (shows all curated posts, weights, URLs)

**Data Files:**
- `_data/collections.yml` - collection definitions & tag fallbacks
- `_data/learning.yml` - learning path definitions & tag fallbacks  
- `_data/ui_text.yml` - copywriting/text labels

## How to Curate

### Featured Hero Post
```yml
home_featured: true
home_featured_order: 10  # Lower number wins
```
Falls back to latest post if none marked.

### Collections
```yml
collections:
  api-integrations: 10      # Order: lower = first
  hardware-devices: 30
  developer-guides: 20
```
**IDs:** `api-integrations`, `hardware-devices`, `developer-guides`  
**Homepage:** Shows 3 posts max per row. Posts appear only once (hero → rows → latest grid).  
**Pages:** `/collections/`, `/collections/<collection-id>`  
**Fallback:** Newest posts matching `tags_fallback` in `_data/collections.yml`

### Learning Paths
```yml
learning:
  hub-flows: 10            # Step order: lower = first
  sap: 20
  ui-design: 30
```
**IDs:** `hub-flows`, `hardware-sensors`, `sap`, `ui-design`  
**Homepage:** 4-card grid with START HERE buttons.  
**Pages:** `/learning/`, `/learning/<learning-id>`  
**Fallback:** Newest posts matching `tags_fallback` in `_data/learning.yml`

## Examples

**Hero + Collection:**
```yml
home_featured: true
home_featured_order: 10
collections:
  api-integrations: 10
```

**Learning Path Step 2:**
```yml
learning:
  hardware-sensors: 20
```

## Notes
- **Ordering:** Use 10, 20, 30… to allow inserting steps later
- **Single placement:** Posts appear only once on homepage (hero → rows → latest grid)
- **Hub-only:** Posts with `collections`/`learning` appear on hub pages even if not on homepage rows


