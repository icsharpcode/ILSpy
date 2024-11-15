/*globals mermaid:false*/
(async () => {
    const getById = id => document.getElementById(id),
        triggerChangeOn = element => { element.dispatchEvent(new Event('change')); },
        hasProperty = (obj, name) => Object.prototype.hasOwnProperty.call(obj, name);

    const checkable = (() => {
        const checked = ':checked',
            inputsByName = name => `input[name=${name}]`,
            getInput = (name, filter, context) => (context || document).querySelector(inputsByName(name) + filter),
            getInputs = (name, context) => (context || document).querySelectorAll(inputsByName(name));

        return {
            getValue: (name, context) => getInput(name, checked, context).value,

            onChange: (name, handle, context) => {
                for (let input of getInputs(name, context)) input.onchange = handle;
            },

            setChecked: (name, value, triggerChange, context) => {
                const input = getInput(name, `[value="${value}"]`, context);
                input.checked = true;
                if (triggerChange !== false) triggerChangeOn(input);
            }
        };
    })();

    const collapse = (() => {
        const open = 'open',
            isOpen = element => element.classList.contains(open),

            /** Toggles the open class on the collapse.
             *  @param {HTMLElement} element The collapse to toggle.
             *  @param {boolean} force The state to force. */
            toggle = (element, force) => element.classList.toggle(open, force);

        return {
            toggle,

            open: element => {
                if (isOpen(element)) return false; // return whether collapse was opened by this process
                return toggle(element, true);
            },

            initToggles: () => {
                for (let trigger of [...document.querySelectorAll('.toggle[href],[data-toggles]')]) {
                    trigger.addEventListener('click', event => {
                        event.preventDefault(); // to avoid pop-state event
                        const trigger = event.currentTarget;
                        trigger.ariaExpanded = !(trigger.ariaExpanded === 'true');
                        toggle(document.querySelector(trigger.attributes.href?.value || trigger.dataset.toggles));
                    });
                }
            }
        };
    })();

    const notify = (() => {
        const toaster = getById('toaster');

        return message => {
            const toast = document.createElement('span');
            toast.innerText = message;
            toaster.appendChild(toast); // fades in the message

            setTimeout(() => {
                toast.classList.add('leaving'); // fades out the message

                // ...and removes it. Note this timeout has to match the animation duration for '.leaving' in the .less file.
                setTimeout(() => { toast.remove(); }, 1000);
            }, 5000);
        };
    })();

    const output = (function () {
        const output = getById('output'),
            hasSVG = () => output.childElementCount > 0,
            getSVG = () => hasSVG() ? output.children[0] : null,

            updateSvgViewBox = (svg, viewBox) => {
                if (svg.originalViewBox === undefined) {
                    const vb = svg.viewBox.baseVal;
                    svg.originalViewBox = { x: vb.x, y: vb.y, width: vb.width, height: vb.height, };
                }

                svg.setAttribute('viewBox', `${viewBox.x} ${viewBox.y} ${viewBox.width} ${viewBox.height}`);
            };

        // enable zooming SVG using Ctrl + mouse wheel
        const zoomFactor = 0.1, panFactor = 2023; // to go with the Zeitgeist

        output.addEventListener('wheel', event => {
            if (!event.ctrlKey || !hasSVG()) return;
            event.preventDefault();

            const svg = getSVG(),
                delta = event.deltaY < 0 ? 1 : -1,
                zoomDelta = 1 + zoomFactor * delta,
                viewBox = svg.viewBox.baseVal;

            viewBox.width *= zoomDelta;
            viewBox.height *= zoomDelta;
            updateSvgViewBox(svg, viewBox);
        });

        // enable panning SVG by grabbing and dragging
        let isPanning = false, panStartX = 0, panStartY = 0;

        output.addEventListener('mousedown', event => {
            isPanning = true;
            panStartX = event.clientX;
            panStartY = event.clientY;
        });

        output.addEventListener('mouseup', () => { isPanning = false; });

        output.addEventListener('mousemove', event => {
            if (!isPanning || !hasSVG()) return;
            event.preventDefault();

            const svg = getSVG(),
                viewBox = svg.viewBox.baseVal,
                dx = event.clientX - panStartX,
                dy = event.clientY - panStartY;

            viewBox.x -= dx * panFactor / viewBox.width;
            viewBox.y -= dy * panFactor / viewBox.height;
            panStartX = event.clientX;
            panStartY = event.clientY;
            updateSvgViewBox(svg, viewBox);
        });

        return {
            getDiagramTitle: () => output.dataset.title,
            setSVG: svg => { output.innerHTML = svg; },
            getSVG,

            resetZoomAndPan: () => {
                const svg = getSVG();
                if (svg !== null) updateSvgViewBox(svg, svg.originalViewBox);
            }
        };
    })();

    const mermaidExtensions = (() => {

        const logLevel = (() => {
            /* int indexes as well as string values can identify a valid log level;
                see log levels and logger definition at https://github.com/mermaid-js/mermaid/blob/develop/packages/mermaid/src/logger.ts .
                Note the names correspond to console output methods https://developer.mozilla.org/en-US/docs/Web/API/console .*/
            const names = ['trace', 'debug', 'info', 'warn', 'error', 'fatal'],
                maxIndex = names.length - 1,

                getIndex = level => {
                    const index = Number.isInteger(level) ? level : names.indexOf(level);
                    return index < 0 ? maxIndex : Math.min(index, maxIndex); // normalize, but return maxIndex (i.e. lowest level) by default
                };

            let requested; // the log level index of the in-coming config or the default

            return {
                /** Sets the desired log level.
                 * @param {string|int} level The name or index of the desired log level. */
                setRequested: level => { requested = getIndex(level); },

                /** Returns all names above (not including) the given level.
                 * @param {int} level The excluded lower boundary log level index (not name).
                 * @returns an array. */
                above: level => names.slice(level + 1),

                /** Indicates whether the log level is configured to be enabled.
                 * @param {string|int} level The log level to test.
                 * @returns a boolean. */
                isEnabled: level => requested <= getIndex(level)
            };
        })();

        /** Calculates the shortest distance in pixels between a point
         *  represented by 'top' and 'left' and the closest side of an axis-aligned rectangle.
         *  Returns 0 if the point is inside or on the edge of the rectangle.
         *  Inspired by https://gamedev.stackexchange.com/a/50722 .
         *  @param {int} top The distance of the point from the top of the viewport.
         *  @param {int} left The distance of the point from the left of the viewport.
         *  @param {DOMRect} rect The bounding box to get the distance to.
         *  @returns {int} The distance of the outside point or 0. */
        function getDistanceToRect(top, left, rect) {
            const dx = Math.max(rect.left, Math.min(left, rect.right)),
                dy = Math.max(rect.top, Math.min(top, rect.bottom));

            return Math.sqrt((left - dx) * (left - dx) + (top - dy) * (top - dy));
        }

        /** Calculates the distance between two non-overlapping axis-aligned rectangles.
         *  Returns 0 if the rectangles touch or overlap.
         *  @param {DOMRect} a The first bounding box.
         *  @param {DOMRect} b The second bounding box.
         *  @returns {int} The distance between the two bounding boxes or 0 if they touch or overlap. */
        function getDistance(a, b) {
            /** Gets coordinate pairs for the corners of a rectangle r.
             * @param {DOMRect} r the rectangle.
             * @returns {Array}} */
            const getCorners = r => [[r.top, r.left], [r.top, r.right], [r.bottom, r.left], [r.bottom, r.right]],
                /** Gets the distances of the corners of rectA to rectB. */
                getCornerDistances = (rectA, rectB) => getCorners(rectA).map(c => getDistanceToRect(c[0], c[1], rectB)),
                aRect = a.getBoundingClientRect(),
                bRect = b.getBoundingClientRect(),
                cornerDistances = getCornerDistances(aRect, bRect).concat(getCornerDistances(bRect, aRect));

            return Math.min(...cornerDistances);
        }

        function interceptConsole(interceptorsByLevel) {
            const originals = {};

            for (let [level, interceptor] of Object.entries(interceptorsByLevel)) {
                if (typeof console[level] !== 'function') continue;
                originals[level] = console[level];
                console[level] = function () { interceptor.call(this, originals[level], arguments); };
            }

            return () => { // call to detach interceptors
                for (let [level, original] of Object.entries(originals))
                    console[level] = original;
            };
        }

        let renderedEdges = [], // contains info about the arrows between types on the diagram once rendered
            lastRenderedDiagram;

        function getRelationLabels(svg, typeId) {
            const edgeLabels = [...svg.querySelectorAll('.edgeLabels span.edgeLabel span')],
                extension = 'extension';

            return renderedEdges.filter(e => e.v === typeId // type name needs to match
                && e.value.arrowTypeStart !== extension && e.value.arrowTypeEnd !== extension) // exclude inheritance arrows
                .map(edge => {
                    const labelHtml = edge.value.label,
                        // filter edge labels with matching HTML
                        labels = edgeLabels.filter(l => l.outerHTML === labelHtml);

                    if (labels.length === 1) return labels[0]; // return the only matching label
                    else if (labels.length < 1) console.error(
                        "Tried to find a relation label for the following edge (by its value.label) but couldn't.", edge);
                    else { // there are multiple edge labels with the same HTML (i.e. matching relation name)
                        // find the path that is rendered for the edge
                        const path = svg.querySelector('.edgePaths>path.relation#' + edge.value.id),
                            labelsByDistance = labels.sort((a, b) => getDistance(path, a) - getDistance(path, b));

                        console.warn('Found multiple relation labels matching the following edge (by its value.label). Returning the closest/first.',
                            edge, labelsByDistance);

                        return labelsByDistance[0]; // and return the matching label closest to it
                    }
                });
        }

        return {
            init: config => {

                /* Override console.info to intercept a message posted by mermaid including information about the edges
                    (represented by arrows between types in the rendered diagram) to access the relationship info
                    parsed from the diagram descriptions of selected types.
                    This works around the mermaid API currently not providing access to this information
                    and it being hard to reconstruct from the rendered SVG alone.
                    Why do we need that info? Knowing about the relationships between types, we can find the label
                    corresponding to a relation and attach XML documentation information to it, if available.
                    See how getRelationLabels is used. */
                const requiredLevel = 2, // to enable intercepting info message

                    interceptors = {
                        info: function (overridden, args) {
                            // intercept message containing rendered edges
                            if (args[2] === 'Graph in recursive render: XXX') renderedEdges = args[3].edges;

                            // only forward to overridden method if this log level was originally enabled
                            if (logLevel.isEnabled(requiredLevel)) overridden.call(this, ...args);
                        }
                    };

                logLevel.setRequested(config.logLevel); // remember original log level

                // lower configured log level if required to guarantee above interceptor gets called
                if (!logLevel.isEnabled(requiredLevel)) config.logLevel = requiredLevel;

                // suppress console output for higher log levels accidentally activated by lowering to required level
                for (let level of logLevel.above(requiredLevel))
                    if (!logLevel.isEnabled(level)) interceptors[level] = () => { };

                const detachInterceptors = interceptConsole(interceptors); // attaches console interceptors
                mermaid.initialize(config); // init the mermaid sub-system with interceptors in place
                detachInterceptors(); // to avoid intercepting messages outside of that context we're not interested in
            },

            /** Processes the type selection into mermaid diagram syntax (and the corresponding XML documentation data, if available).
             * @param {object} typeDetails An object with the IDs of types to display in detail (i.e. with members) for keys
             * and objects with the data structure of ClassDiagrammer.Type (excluding the Id) for values.
             * @param {function} getTypeLabel A strategy for getting the type label for a type ID.
             * @param {string} direction The layout direction of the resulting diagram.
             * @param {object} showInherited A regular expression matching things to exclude from the diagram definition.
             * @returns {object} An object like { diagram, detailedTypes, xmlDocs } with 'diagram' being the mermaid diagram syntax,
             * 'xmlDocs' the corresponding XML documentation to be injected into the rendered diagram in the 'postProcess' step and
             * 'detailedTypes' being a flat list of IDs of types that will be rendered in detail (including their members and relations). */
            processTypes: (typeDetails, getTypeLabel, direction, showInherited) => {
                const detailedTypes = Object.keys(typeDetails), // types that will be rendered including their members and relations
                    xmlDocs = {}, // to be appended with docs of selected types below
                    getAncestorTypes = typeDetails => Object.keys(typeDetails.Inherited),
                    isRendered = type => detailedTypes.includes(type),

                    mayNeedLabelling = new Set(),

                    cleanUpDiagramMmd = mmd => mmd.replace(/(\r?\n){3,}/g, '\n\n'), // squash more than two consecutive line breaks down into two

                    // renders base type and interfaces depending on settings and selected types
                    renderSuperType = (supertTypeId, link, typeId, name, displayAll) => {
                        /* display relation arrow if either the user chose to display this kind of super type
                            or the super type is selected to be rendered anyway and we might as well for completeness */
                        if (displayAll || isRendered(supertTypeId)) {
                            const label = name ? ' : ' + name : '';
                            diagram += `${supertTypeId} <|${link} ${typeId}${label}\n`;
                            mayNeedLabelling.add(supertTypeId);
                        }
                    },

                    /*  TODO watch https://github.com/mermaid-js/mermaid/issues/6034 for a solution to render multiple self-references,
                        which is currently broken. E.g. for LightJson.JsonValue (compare console log) */
                    // renders HasOne and HasMany relations
                    renderRelations = (typeId, relations, many) => {
                        if (relations) // expecting object; only process if not null or undefined
                            for (let [label, relatedId] of Object.entries(relations)) {
                                const nullable = label.endsWith(' ?');
                                const cardinality = many ? '"*" ' : nullable ? '"?" ' : '';
                                if (nullable) label = label.substring(0, label.length - 2); // nullability is expressed via cardinality
                                diagram += `${typeId} --> ${cardinality}${relatedId} : ${label}\n`;
                                mayNeedLabelling.add(relatedId);
                            }
                    },

                    renderInheritedMembers = (typeId, details) => {
                        const ancestorTypes = getAncestorTypes(details);

                        // only include inherited members in sub classes if they aren't already rendered in a super class
                        for (let [ancestorType, members] of Object.entries(details.Inherited)) {
                            if (isRendered(ancestorType)) continue; // inherited members will be rendered in base type

                            let ancestorsOfDetailedAncestors = ancestorTypes.filter(t => detailedTypes.includes(t)) // get detailed ancestor types
                                .map(type => getAncestorTypes(typeDetails[type])) // select their ancestor types
                                .reduce((union, ancestors) => union.concat(ancestors), []); // squash them into a one-dimensional array (ignoring duplicates)

                            // skip displaying inherited members already displayed by detailed ancestor types
                            if (ancestorsOfDetailedAncestors.includes(ancestorType)) continue;

                            diagram += members.FlatMembers + '\n';
                            renderRelations(typeId, members.HasOne);
                            renderRelations(typeId, members.HasMany, true);
                        }
                    };

                // init diagram code with header and layout direction to be appended to below
                let diagram = 'classDiagram' + '\n'
                    + 'direction ' + direction + '\n\n';

                // process selected types
                for (let [typeId, details] of Object.entries(typeDetails)) {
                    mayNeedLabelling.add(typeId);
                    diagram += details.Body + '\n\n';

                    if (details.BaseType) // expecting object; only process if not null or undefined
                        for (let [baseTypeId, label] of Object.entries(details.BaseType))
                            renderSuperType(baseTypeId, '--', typeId, label, showInherited.types);

                    if (details.Interfaces) // expecting object; only process if not null or undefined
                        for (let [ifaceId, labels] of Object.entries(details.Interfaces))
                            for (let label of labels)
                                renderSuperType(ifaceId, '..', typeId, label, showInherited.interfaces);

                    renderRelations(typeId, details.HasOne);
                    renderRelations(typeId, details.HasMany, true);
                    xmlDocs[typeId] = details.XmlDocs;
                    if (showInherited.members && details.Inherited) renderInheritedMembers(typeId, details);
                }

                for (let typeId of mayNeedLabelling) {
                    const label = getTypeLabel(typeId);
                    if (label !== typeId) diagram += `class ${typeId} ["${label}"]\n`;
                }

                diagram = cleanUpDiagramMmd(diagram);
                lastRenderedDiagram = diagram; // store diagram syntax for export
                return { diagram, detailedTypes, xmlDocs };
            },

            getDiagram: () => lastRenderedDiagram,

            /** Enhances the SVG rendered by mermaid by injecting xmlDocs if available
             * and attaching type click handlers, if available.
             * @param {SVGElement} svg The SVG containing the rendered mermaid diagram.
             * @param {object} options An object like { xmlDocs, onTypeClick }
             * with 'xmlDocs' being the XML docs by type ID
             * and 'onTypeClick' being an event listener for the click event
             * that gets the event and the typeId as parameters. */
            postProcess: (svg, options) => {
                // matches 'MyClass2' from generated id attributes in the form of 'classId-MyClass2-0'
                const typeIdFromDomId = /(?<=classId-)\w+(?=-\d+)/;

                for (let entity of svg.querySelectorAll('g.nodes>g.node').values()) {
                    const typeId = typeIdFromDomId.exec(entity.id)[0];

                    // clone to have a modifiable collection without affecting the original
                    const docs = structuredClone((options.xmlDocs || [])[typeId]);

                    // splice in XML documentation as label titles if available
                    if (docs) {
                        const typeKey = '', nodeLabel = 'span.nodeLabel',
                            title = entity.querySelector('.label-group'),
                            relationLabels = getRelationLabels(svg, typeId),

                            setDocs = (label, member) => {
                                label.title = docs[member];
                                delete docs[member];
                            },

                            documentOwnLabel = (label, member) => {
                                setDocs(label, member);
                                ownLabels = ownLabels.filter(l => l !== label); // remove label
                            };

                        let ownLabels = [...entity.querySelectorAll('g.label ' + nodeLabel)];

                        // document the type label itself
                        if (hasProperty(docs, typeKey)) documentOwnLabel(title.querySelector(nodeLabel), typeKey);

                        // loop through documented members longest name first
                        for (let member of Object.keys(docs).sort((a, b) => b.length - a.length)) {
                            // matches only whole words in front of method signatures starting with (
                            const memberName = new RegExp(`(?<!.*\\(.*)\\b${member}\\b`),
                                matchingLabels = ownLabels.filter(l => memberName.test(l.textContent)),
                                related = relationLabels.find(l => l.textContent === member);

                            if (related) matchingLabels.push(related);
                            if (matchingLabels.length === 0) continue; // members may be rendered in an ancestor type

                            if (matchingLabels.length > 1) console.warn(
                                `Expected to find one member or relation label for ${title.textContent}.${member}`
                                + ' to attach the XML documentation to but found multiple. Applying the first.', matchingLabels);

                            documentOwnLabel(matchingLabels[0], member);
                        }
                    }

                    if (typeof options.onTypeClick === 'function') entity.addEventListener('click',
                        function (event) { options.onTypeClick.call(this, event, typeId); });
                }
            }
        };
    })();

    const state = (() => {
        const typeUrlDelimiter = '-',
            originalTitle = document.head.getElementsByTagName('title')[0].textContent;

        const restore = async data => {
            if (data.d) layoutDirection.set(data.d);

            if (data.t) {
                inheritanceFilter.setFlagHash(data.i || ''); // if types are set, enable deselecting all options
                typeSelector.setSelected(data.t.split(typeUrlDelimiter));
                await render(true);
            }
        };

        function updateQueryString(href, params) {
            // see https://developer.mozilla.org/en-US/docs/Web/API/URL
            const url = new URL(href), search = url.searchParams;

            for (const [name, value] of Object.entries(params)) {
                //see https://developer.mozilla.org/en-US/docs/Web/API/URLSearchParams
                if (value === null || value === undefined || value === '') search.delete(name);
                else if (Array.isArray(value)) {
                    search.delete(name);
                    for (let item of value) search.append(name, item);
                }
                else search.set(name, value);
            }

            url.search = search.toString();
            return url.href;
        }

        window.onpopstate = async event => { await restore(event.state); };

        return {
            update: () => {
                const types = typeSelector.getSelected(),
                    t = Object.keys(types).join(typeUrlDelimiter),
                    d = layoutDirection.get(),
                    i = inheritanceFilter.getFlagHash(),
                    data = { t, d, i },
                    typeNames = Object.values(types).map(t => t.Name);

                history.pushState(data, '', updateQueryString(location.href, data));

                // record selected types in title so users see which selection they return to when using a history link
                document.title = (typeNames.length ? typeNames.join(', ') + ' - ' : '') + originalTitle;
            },
            restore: async () => {
                if (!location.search) return; // assume fresh open and don't try to restore state, preventing inheritance options from being unset
                const search = new URLSearchParams(location.search);
                await restore({ d: search.get('d'), i: search.get('i'), t: search.get('t') });
            }
        };
    })();

    const typeSelector = (() => {
        const select = getById('type-select'),
            preFilter = getById('pre-filter-types'),
            renderBtn = getById('render'),
            model = JSON.parse(getById('model').innerHTML),
            tags = { optgroup: 'OPTGROUP', option: 'option' },
            getNamespace = option => option.parentElement.nodeName === tags.optgroup ? option.parentElement.label : '',
            getOption = typeId => select.querySelector(tags.option + `[value='${typeId}']`);

        // fill select list
        for (let [namespace, types] of Object.entries(model.TypesByNamespace)) {
            let optionParent;

            if (namespace) {
                const group = document.createElement(tags.optgroup);
                group.label = namespace;
                select.appendChild(group);
                optionParent = group;
            } else optionParent = select;

            for (let typeId of Object.keys(types)) {
                const type = types[typeId],
                    option = document.createElement(tags.option);

                option.value = typeId;
                if (!type.Name) type.Name = typeId; // set omitted label to complete structure
                option.innerText = type.Name;
                optionParent.appendChild(option);
            }
        }

        // only enable render button if types are selected
        select.onchange = () => { renderBtn.disabled = select.selectedOptions.length < 1; };

        preFilter.addEventListener('input', () => {
            const regex = preFilter.value ? new RegExp(preFilter.value, 'i') : null;

            for (let option of select.options)
                option.hidden = regex !== null && !regex.test(option.innerHTML);

            // toggle option groups hidden depending on whether they have visible children
            for (let group of select.getElementsByTagName(tags.optgroup))
                group.hidden = regex !== null && [...group.children].filter(o => !o.hidden).length === 0;
        });

        return {
            focus: () => select.focus(),
            focusFilter: () => preFilter.focus(),

            setSelected: types => {
                for (let option of select.options)
                    option.selected = types.includes(option.value);

                triggerChangeOn(select);
            },

            toggleOption: typeId => {
                const option = getOption(typeId);

                if (option !== null) {
                    option.selected = !option.selected;
                    triggerChangeOn(select);
                }
            },

            /** Returns the types selected by the user in the form of an object with the type IDs for keys
             *  and objects with the data structure of ClassDiagrammer.Type (excluding the Id) for values. */
            getSelected: () => Object.fromEntries([...select.selectedOptions].map(option => {
                const namespace = getNamespace(option), typeId = option.value,
                    details = model.TypesByNamespace[namespace][typeId];

                return [typeId, details];
            })),

            moveSelection: up => {
                // inspired by https://stackoverflow.com/a/25851154
                for (let option of select.selectedOptions) {
                    if (up && option.previousElementSibling) { // move up
                        option.parentElement.insertBefore(option, option.previousElementSibling);
                    } else if (!up && option.nextElementSibling) { // move down
                        // see https://developer.mozilla.org/en-US/docs/Web/API/Node/insertBefore
                        option.parentElement.insertBefore(option, option.nextElementSibling.nextElementSibling);
                    }
                }
            },

            //TODO add method returning namespace to add to title
            getLabel: typeId => {
                const option = getOption(typeId);
                return option ? option.innerText : model.OutsideReferences[typeId];
            }
        };
    })();

    const inheritanceFilter = (() => {
        const baseType = getById('show-base-types'),
            interfaces = getById('show-interfaces'),
            members = getById('show-inherited-members'),
            getFlags = () => { return { types: baseType.checked, interfaces: interfaces.checked, members: members.checked }; };

        // automatically re-render on change
        for (let checkbox of [baseType, interfaces, members])
            checkbox.onchange = async () => { await render(); };

        return {
            getFlags,

            getFlagHash: () => Object.entries(getFlags())
                .filter(([, value]) => value) // only true flags
                .map(([key]) => key[0]).join(''), // first character of each flag

            setFlagHash: hash => {
                baseType.checked = hash.includes('t');
                interfaces.checked = hash.includes('i');
                members.checked = hash.includes('m');
            }
        };
    })();

    const layoutDirection = (() => {
        const inputName = 'direction';

        // automatically re-render on change
        checkable.onChange(inputName, async () => { await render(); });

        return {
            get: () => checkable.getValue(inputName),
            set: (value, event) => {
                const hasEvent = event !== undefined;
                checkable.setChecked(inputName, value, hasEvent);
                if (hasEvent) event.preventDefault();
            }
        };
    })();

    const render = async isRestoringState => {
        const { diagram, detailedTypes, xmlDocs } = mermaidExtensions.processTypes(
            typeSelector.getSelected(), typeSelector.getLabel, layoutDirection.get(), inheritanceFilter.getFlags());

        console.info(diagram);
        const titledDiagram = diagram + '\naccTitle: ' + output.getDiagramTitle().replaceAll('\n', '#10;') + '\n';

        /* Renders response and deconstructs returned object because we're only interested in the svg.
            Note that the ID supplied as the first argument must not match any existing element ID
            unless you want its contents to be replaced. See https://mermaid.js.org/config/usage.html#api-usage */
        const { svg } = await mermaid.render('foo', titledDiagram);
        output.setSVG(svg);

        mermaidExtensions.postProcess(output.getSVG(), {
            xmlDocs,

            onTypeClick: async (event, typeId) => {
                // toggle selection and re-render on clicking entity
                typeSelector.toggleOption(typeId);
                await render();
            }
        });

        exportOptions.enable(detailedTypes.length > 0);
        if (!isRestoringState) state.update();
    };

    const filterSidebar = (() => {
        const filterForm = getById('filter'),
            resizing = 'resizing',
            toggleBtn = getById('filter-toggle'),
            toggle = () => collapse.toggle(filterForm);

        // enable rendering by hitting Enter on filter form
        filterForm.onsubmit = async (event) => {
            event.preventDefault();
            await render();
        };

        // enable adjusting max sidebar width
        (() => {
            const filterWidthOverride = getById('filter-width'), // a style tag dedicated to overriding the default filter max-width
                minWidth = 210, maxWidth = window.innerWidth / 2; // limit the width of the sidebar

            let isDragging = false; // tracks whether the sidebar is being dragged
            let pickedUp = 0; // remembers where the dragging started from
            let widthBefore = 0; // remembers the width when dragging starts
            let change = 0; // remembers the total distance of the drag

            toggleBtn.addEventListener('mousedown', (event) => {
                isDragging = true;
                pickedUp = event.clientX;
                widthBefore = filterForm.offsetWidth;
            });

            document.addEventListener('mousemove', (event) => {
                if (!isDragging) return;

                const delta = event.clientX - pickedUp,
                    newWidth = Math.max(minWidth, Math.min(maxWidth, widthBefore + delta));

                change = delta;
                filterForm.classList.add(resizing);
                filterWidthOverride.innerHTML = `#filter.open { max-width: ${newWidth}px; }`;
            });

            document.addEventListener('mouseup', () => {
                if (!isDragging) return;
                isDragging = false;
                filterForm.classList.remove(resizing);
            });

            // enable toggling filter info on click
            toggleBtn.addEventListener('click', () => {
                if (Math.abs(change) < 5) toggle(); // prevent toggling for small, accidental drags
                change = 0; // reset the remembered distance to enable subsequent clicks
            });
        })();

        return {
            toggle,
            open: () => collapse.open(filterForm)
        };
    })();

    /* Shamelessly copied from https://github.com/mermaid-js/mermaid-live-editor/blob/develop/src/lib/components/Actions.svelte
        with only a few modifications after I failed to get the solutions described here working:
        https://stackoverflow.com/questions/28226677/save-inline-svg-as-jpeg-png-svg/28226736#28226736
        The closest I got was with this example https://canvg.js.org/examples/offscreen , but the shapes would remain empty. */
    const exporter = (() => {
        const getSVGstring = (svg, width, height) => {
            height && svg?.setAttribute('height', `${height}px`);
            width && svg?.setAttribute('width', `${width}px`); // Workaround https://stackoverflow.com/questions/28690643/firefox-error-rendering-an-svg-image-to-html5-canvas-with-drawimage
            if (!svg) svg = getSvgEl();

            return svg.outerHTML.replaceAll('<br>', '<br/>')
                .replaceAll(/<img([^>]*)>/g, (m, g) => `<img ${g} />`);
        };

        const toBase64 = utf8String => {
            const bytes = new TextEncoder().encode(utf8String);
            return window.btoa(String.fromCharCode.apply(null, bytes));
        };

        const getBase64SVG = (svg, width, height) => toBase64(getSVGstring(svg, width, height));

        const exportImage = (event, exporter, imagemodeselected, userimagesize) => {
            const canvas = document.createElement('canvas');
            const svg = document.querySelector('#output svg');
            if (!svg) {
                throw new Error('svg not found');
            }
            const box = svg.getBoundingClientRect();
            canvas.width = box.width;
            canvas.height = box.height;
            if (imagemodeselected === 'width') {
                const ratio = box.height / box.width;
                canvas.width = userimagesize;
                canvas.height = userimagesize * ratio;
            } else if (imagemodeselected === 'height') {
                const ratio = box.width / box.height;
                canvas.width = userimagesize * ratio;
                canvas.height = userimagesize;
            }
            const context = canvas.getContext('2d');
            if (!context) {
                throw new Error('context not found');
            }
            context.fillStyle = 'white';
            context.fillRect(0, 0, canvas.width, canvas.height);
            const image = new Image();
            image.onload = exporter(context, image);
            image.src = `data:image/svg+xml;base64,${getBase64SVG(svg, canvas.width, canvas.height)}`;
            event.stopPropagation();
            event.preventDefault();
        };

        const getSvgEl = () => {
            const svgEl = document.querySelector('#output svg').cloneNode(true);
            svgEl.setAttribute('xmlns:xlink', 'http://www.w3.org/1999/xlink');
            const fontAwesomeCdnUrl = Array.from(document.head.getElementsByTagName('link'))
                .map((l) => l.href)
                .find((h) => h.includes('font-awesome'));
            if (fontAwesomeCdnUrl == null) {
                return svgEl;
            }
            const styleEl = document.createElement('style');
            styleEl.innerText = `@import url("${fontAwesomeCdnUrl}");'`;
            svgEl.prepend(styleEl);
            return svgEl;
        };

        const simulateDownload = (download, href) => {
            const a = document.createElement('a');
            a.download = download;
            a.href = href;
            a.click();
            a.remove();
        };

        const downloadImage = (context, image) => {
            return () => {
                const { canvas } = context;
                context.drawImage(image, 0, 0, canvas.width, canvas.height);
                simulateDownload(
                    exportOptions.getFileName('png'),
                    canvas.toDataURL('image/png').replace('image/png', 'image/octet-stream')
                );
            };
        };

        const tryWriteToClipboard = blob => {
            try {
                if (!blob) throw new Error('blob is empty');
                void navigator.clipboard.write([new ClipboardItem({ [blob.type]: blob })]);
                return true;
            } catch (error) {
                console.error(error);
                return false;
            }
        };

        const copyPNG = (context, image) => {
            return () => {
                const { canvas } = context;
                context.drawImage(image, 0, 0, canvas.width, canvas.height);
                canvas.toBlob(blob => { tryWriteToClipboard(blob); });
            };
        };

        const tryWriteTextToClipboard = async text => {
            try {
                if (!text) throw new Error('text is empty');
                await navigator.clipboard.writeText(text);
                return true;
            } catch (error) {
                console.error(error);
                return false;
            }
        };

        const copyText = async (event, text) => {
            if (await tryWriteTextToClipboard(text)) {
                event.stopPropagation();
                event.preventDefault();
            }
        };

        return {
            isClipboardAvailable: () => hasProperty(window, 'ClipboardItem'),
            onCopyPNG: (event, imagemodeselected, userimagesize) => {
                exportImage(event, copyPNG, imagemodeselected, userimagesize);
            },
            onCopySVG: event => { void copyText(event, getSVGstring()); },
            onCopyMMD: (event, diagram) => { void copyText(event, diagram); },
            onDownloadPNG: (event, imagemodeselected, userimagesize) => {
                exportImage(event, downloadImage, imagemodeselected, userimagesize);
            },
            onDownloadSVG: () => {
                simulateDownload(exportOptions.getFileName('svg'), `data:image/svg+xml;base64,${getBase64SVG()}`);
            },
            onDownloadMMD: diagram => {
                simulateDownload(exportOptions.getFileName('mmd'), `data:text/vnd.mermaid;base64,${toBase64(diagram)}`);
            }
        };
    })();

    const exportOptions = (() => {
        let wereOpened = false; // used to track whether user was able to see save options and may quick-save

        const container = getById('exportOptions'),
            toggle = getById('exportOptions-toggle'),
            saveBtn = getById('save'),
            copyBtn = getById('copy'),
            saveAs = 'saveAs',
            png = 'png',
            svg = 'svg',
            isDisabled = () => toggle.hidden, // using toggle visibility as indicator

            open = () => {
                wereOpened = true;
                return collapse.open(container);
            },

            copy = event => {
                if (isDisabled()) return; // allow the default for copying text if no types are rendered

                if (!exporter.isClipboardAvailable()) notify('The clipboard seems unavailable in this browser :(');
                else {
                    const type = checkable.getValue(saveAs);

                    try {
                        if (type === png) {
                            const [dimension, size] = getDimensions();
                            exporter.onCopyPNG(event, dimension, size);
                        }
                        else if (type === svg) exporter.onCopySVG(event);
                        else exporter.onCopyMMD(event, mermaidExtensions.getDiagram());

                        notify(`The diagram ${type.toUpperCase()} is in your clipboard.`);
                    } catch (e) {
                        notify(e.toString());
                    }
                }
            },

            save = event => {
                const type = checkable.getValue(saveAs);

                if (type === png) {
                    const [dimension, size] = getDimensions();
                    exporter.onDownloadPNG(event, dimension, size);
                }
                else if (type === svg) exporter.onDownloadSVG();
                else exporter.onDownloadMMD(mermaidExtensions.getDiagram());
            };

        const getDimensions = (() => {
            const inputName = 'dimension',
                scale = 'scale',
                dimensions = getById('dimensions'),
                scaleInputs = container.querySelectorAll('#scale-controls input');

            // enable toggling dimension controls
            checkable.onChange(saveAs, event => {
                collapse.toggle(dimensions, event.target.value === png);
            }, container);

            // enable toggling scale controls
            checkable.onChange(inputName, event => {
                const disabled = event.target.value !== scale;
                for (let input of scaleInputs) input.disabled = disabled;
            }, container);

            return () => {
                let dimension = checkable.getValue(inputName);

                // return dimension to scale to desired size if not exporting in current size
                if (dimension !== 'auto') dimension = checkable.getValue(scale);

                return [dimension, getById('scale-size').value];
            };
        })();

        if (exporter.isClipboardAvailable()) copyBtn.onclick = copy;
        else copyBtn.hidden = true;

        saveBtn.onclick = save;

        return {
            copy,
            getFileName: ext => `${saveBtn.dataset.assembly}-diagram-${new Date().toISOString().replace(/[Z:.]/g, '')}.${ext}`,

            enable: enable => {
                if (!enable) collapse.toggle(container, false); // make sure the container is closed when disabling
                toggle.hidden = !enable;
            },

            quickSave: event => {
                if (isDisabled()) return; // allow the default for saving HTML doc if no types are rendered

                if (wereOpened) {
                    save(event); // allow quick save
                    return;
                }

                const filterOpened = filterSidebar.open(),
                    optionsOpenend = open();

                /* Make sure the collapses containing the save options are open and visible when user hits Ctrl + S.
                    If neither needed opening, trigger saving. I.e. hitting Ctrl + S again should do it. */
                if (!filterOpened && !optionsOpenend) save(event);
                else event.preventDefault(); // prevent saving HTML page
            }
        };
    })();

    // displays pressed keys and highlights mouse cursor for teaching usage and other presentations
    const controlDisplay = (function () {
        let used = new Set(), enabled = false, wheelTimeout;

        const alt = 'Alt',
            display = getById('pressed-keys'), // a label displaying the keys being pressed and mouse wheel being scrolled
            mouse = getById('mouse'), // a circle tracking the mouse to make following it easier

            translateKey = key => key.length === 1 ? key.toUpperCase() : key,

            updateDisplay = () => {
                display.textContent = [...used].join(' + ');
                display.classList.toggle('hidden', used.size === 0);
            },

            eventHandlers = {
                keydown: event => {
                    if (event.altKey) used.add(alt); // handle separately because Alt key alone doesn't trigger a key event
                    used.add(translateKey(event.key));
                    updateDisplay();
                },

                keyup: event => {
                    setTimeout(() => {
                        if (!event.altKey && used.has(alt)) used.delete(alt);
                        used.delete(translateKey(event.key));
                        updateDisplay();
                    }, 500);
                },

                wheel: event => {
                    const label = 'wheel ' + (event.deltaY < 0 ? 'up' : 'down'),
                        wasUsed = used.has(label);

                    if (wasUsed) {
                        if (wheelTimeout) clearTimeout(wheelTimeout);
                    } else {
                        used.add(label);
                        updateDisplay();
                    }

                    // automatically remove
                    wheelTimeout = setTimeout(() => {
                        used.delete(label);
                        updateDisplay();
                        wheelTimeout = undefined;
                    }, 500);
                },

                mousemove: event => {
                    mouse.style.top = event.clientY + 'px';
                    mouse.style.left = event.clientX + 'px';
                },

                mousedown: () => { mouse.classList.add('down'); },
                mouseup: () => { setTimeout(() => { mouse.classList.remove('down'); }, 300); }
            };

        return {
            toggle: () => {
                enabled = !enabled;

                if (enabled) {
                    mouse.hidden = false;

                    for (let [event, handler] of Object.entries(eventHandlers))
                        document.addEventListener(event, handler);
                } else {
                    mouse.hidden = true;

                    for (let [event, handler] of Object.entries(eventHandlers))
                        document.removeEventListener(event, handler);

                    used.clear();
                    updateDisplay();
                }
            }
        };
    })();

    // key bindings
    document.onkeydown = async (event) => {
        const arrowUp = 'ArrowUp', arrowDown = 'ArrowDown';

        // support Cmd key as alternative on Mac, see https://stackoverflow.com/a/5500536
        if (event.ctrlKey || event.metaKey) {
            switch (event.key) {
                case 'b': filterSidebar.toggle(); return;
                case 'k':
                    event.preventDefault();
                    filterSidebar.open();
                    typeSelector.focusFilter();
                    return;
                case 's': exportOptions.quickSave(event); return;
                case 'c': exportOptions.copy(event); return;
                case 'i':
                    event.preventDefault();
                    controlDisplay.toggle();
                    return;
                case 'ArrowLeft': layoutDirection.set('RL', event); return;
                case 'ArrowRight': layoutDirection.set('LR', event); return;
                case arrowUp: layoutDirection.set('BT', event); return;
                case arrowDown: layoutDirection.set('TB', event); return;
                case '0': output.resetZoomAndPan(); return;
            }
        }

        if (event.altKey) { // naturally triggered by Mac's option key as well
            // enable moving selected types up and down using arrow keys while holding [Alt]
            const upOrDown = event.key === arrowUp ? true : event.key === arrowDown ? false : null;

            if (upOrDown !== null) {
                typeSelector.focus();
                typeSelector.moveSelection(upOrDown);
                event.preventDefault();
                return;
            }

            // pulse-animate elements with helping title attributes to point them out
            if (event.key === 'i') {
                event.preventDefault();
                const pulsing = 'pulsing';

                for (let element of document.querySelectorAll('[title],:has(title)')) {
                    element.addEventListener('animationend', () => { element.classList.remove(pulsing); }, { once: true });
                    element.classList.add(pulsing);
                }
            }
        }
    };

    // rewrite help replacing references to 'Ctrl' with 'Cmd' for Mac users
    if (/(Mac)/i.test(navigator.userAgent)) {
        const ctrl = /Ctrl/mg,
            replace = source => source.replaceAll(ctrl, '⌘');

        for (let titled of document.querySelectorAll('[title]'))
            if (ctrl.test(titled.title)) titled.title = replace(titled.title);

        for (let titled of document.querySelectorAll('[data-title]'))
            if (ctrl.test(titled.dataset.title)) titled.dataset.title = replace(titled.dataset.title);

        for (let element of getById('info').querySelectorAll('*')) {
            const text = element.innerText || element.textContent; // Get the text content of the element
            if (ctrl.test(text)) element.innerHTML = replace(text);
        }
    }

    collapse.initToggles();
    mermaidExtensions.init({ startOnLoad: false }); // initializes mermaid as well
    typeSelector.focus(); // focus type filter initially to enable keyboard input
    await state.restore();
})();
