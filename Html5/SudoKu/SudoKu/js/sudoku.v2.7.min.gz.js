var texts = {
    share: "Copy the link below to share the currently displayed board", exp: "This string represent the board, with '0' as empty cell", imp: "Enter a string of 81 characters (blanks can be 0, or anything but a number)", import_invalid: "Your submission is invalid, please check again", board_invalid: "Invalid board: no single solution found", saveGame: "Game saved", loadGame: "Game loaded", newGame: "New game loaded", win: "You won! very impressive", loose: "Solution is not valid, please verify", pause: "Game paused",
    clear_cell: "Clear cell", no_games_in_level: "No games for this level, try again", no_solution: "Solution is missing"
}, options = {
    button: {
        levelMenu: $("header > div.output > ul"), undo: $("#options > button.undo"), restart: $("#options > button.restart"), newGame: $("#options > button.new"), solution: $("#options > button.solution").data("state", 0), notes: $("#options > button.notes"), note: $("#options > button.note"), clearNotes: $("#options > button.clearNotes"), save: $("#options_advanced > button.save"), load: $("#options_advanced > button.load"),
        Import: $("#options_advanced > button.import"), Export: $("#options_advanced > button.export"), share: $("#options_advanced > button.share"), set: function (a, b) { this[a].prop("disabled", b ? !1 : !0) }
    }, bind_buttons: function () {
        var a = this; $("#options > button").prop("disabled", !1); this.button.undo.bind("click", a.undo); this.undoToggle(); this.button.restart.bind("click", $.proxy(a, "restart")); this.button.newGame.bind("click", $.proxy(a, "newGame")); this.button.solution.bind("click", $.proxy(a, "solution")); this.button.Import.bind("click",
        function () { a.Import() }); this.button.Export.bind("click", function () { a.Export() }); this.button.share.bind("click", $.proxy(a, "share")); this.button.notes.bind("click", function () { a.candidates.set() }); this.button.note.bind("click", function () { a.notes.set() }); this.button.clearNotes.bind("click", function () { a.notes.remove() }); utility.support.storage ? (this.button.save.bind("click", function () { a.save() }), this.button.load.bind("click", function () { a.load() }), localStorage && this.button.save.add(this.button.load).show(),
        localStorage && localStorage["sudoku.arr"] || this.button.load.prop("disabled", !0)) : this.button.save.add(this.button.load).hide(); this.levelMenu()
    }, levelMenu: function () { var a = this.button.levelMenu; a.find("li").filter(function () { return $(this).text() == board.level }).addClass("on"); a.on("click", "li", function (a) { a.stopPropagation(); $(this).addClass("on").siblings(".on").removeClass("on"); board.level = this.innerHTML; board.loadGameByLevel(board.level) }) }, reset: function (a) {
        a || (board.movesHistory.length = 0, board.timer.restart());
        this.candidates.remove(); this.notes.remove(); this.undoToggle(); options.button.solution.data("state", 0).removeClass("on")
    }, undo: function () { board.changeCell(); board.movesHistory.splice(-1); options.undoToggle() }, undoToggle: function () { options.button.set("undo", 0 < board.movesHistory.length ? !0 : !1) }, restart: function () { this.reset(); board.arr = $.extend(!0, [], board.arr_original); board.userCells.find("> div span:first").empty(); board.numberOfMoves = utility.countEmtpyCells() }, newGame: function () {
        window.location.search &&
        (window.location.search = ""); this.reset(); options.button.set("newGame", 0); board.loadGameByLevel(board.level); utility.alert.say(texts.newGame + " (" + solver.score() + ")"); options.button.set("newGame", 1)
    }, solution: function () {
        var a = options.button.solution, b, c; if (board.solution && "string" == typeof board.solution) {
            c = a.data("state"); b = utility.decodeStr(board.solution); if (c) { for (var d in b) board.$cellsArr[d].removeClass("filled"); b = utility.getDiffArrays(utility.flattenBoard(board.arr), utility.flattenBoard(board.arr_original)) } board.renderCellValues(b);
            a.data("state", !c).toggleClass("on")
        } else utility.alert.say(texts.no_solution), a.data("state", 0).removeClass("on")
    }, save: function () { localStorage["sudoku.arr"] = JSON.stringify(board.arr); localStorage["sudoku.gameStr"] = board.solution; localStorage["sudoku.movesHistory"] = JSON.stringify(board.movesHistory); localStorage["sudoku.score"] = board.level; localStorage["sudoku.time"] = board.timer.hour + ":" + board.timer.min + ":" + board.timer.sec; this.button.load.prop("disabled", !1); utility.alert.say(texts.saveGame) }, load: function () {
        this.reset(!0);
        board.arr = JSON.parse(localStorage["sudoku.arr"]); board.solution = localStorage["sudoku.gameStr"]; board.movesHistory = JSON.parse(localStorage["sudoku.movesHistory"]); solver.setScore(localStorage["sudoku.score"]); board.solution = board.solution; var a = {}, b; for (b = board.movesHistory.length; b--;) a[board.movesHistory[b][0]] = 1; board.numberOfMoves = utility.countEmtpyCells(); board.load({ arr: board.arr, filledCells: a }); board.timer.paused && board.timer.el.trigger("click"); board.timer.set(localStorage["sudoku.time"]).go(!0);
        utility.alert.say(texts.loadGame)
    }, share: function () { var a = window.location.host + "?" + utility.flattenBoard(board.arr_original).join(""); prompt(texts.share, a) }, Export: function () { prompt(texts.exp, board.solution) }, Import: function (a) {
        if (a = a || prompt(texts.imp)) if (81 != a.length) utility.alert.shout(texts.import_invalid); else {
            var b = [], c; for (c = 0; 9 > c; c++) { var d = a.substr(9 * c, 9).split(""), e; for (e in d) d[e] |= 0; b.push(d) } 1 != dlx.solve($.extend(!0, [], b)) && utility.alert.shout(texts.board_invalid); b = utility.transformArray(b);
            this.reset(); board.solution = a; board.load({ arr: b }); board.numberOfMoves = utility.countEmtpyCells(); solver.init(b).step(0); solver.setScore()
        }
    }, notes: {
        possibilities: {}, toggle: !1, set: function () { this.toggle = !this.toggle; options.button.note.toggleClass("on", this.toggle); !this.toggle && options.candidates.toggle && options.candidates.set() }, update: function (a, b) { var c = this.possibilities; if (a in c) { var d = c[a].indexOf(b); -1 != d ? c[a].splice(d, 1) : c[a].push(b) } else c[a] = [b]; c[a].sort() }, check: function (a, b) {
            a && (delete this.possibilities[b],
            board.userCells.filter(function () { return $(this).data("pos") == b }).find("table.candidates td").empty(), this.possibilities = analyzer.evaluatePosib.apply(this, [board]), this.render())
        }, render: function () { var a = this.possibilities, b, c, d; for (b in a) for (c = board.userCells.not(".filled").filter(function () { return $(this).data("pos") == b }), d = c.find("table.candidates td").empty(), c = 0; 9 > c; c++) d.eq(a[b][c] - 1).html(a[b][c]) }, remove: function () {
            board.userCells.length && (this.possibilities = {}, options.candidates.toggle &&
            options.candidates.set(), board.userCells.find("table.candidates td").empty())
        }
    }, candidates: {
        toggle: !1, set: function () { this.toggle ? this.remove() : (options.button.notes.addClass("on"), options.notes.toggle && options.notes.set(), this.render()) }, render: function () { var a, b, c, d; this.toggle = !0; a = analyzer.evaluateTotal(board); board.userCells.not(".filled").each(function () { c = $(this).data("pos"); b = $(this).find("table.candidates td"); b.empty(); for (d = 0; 9 > d; d++) b.eq(a[c][d] - 1).text(a[c][d]) }) }, remove: function () {
            board.userCells.length &&
            (options.button.notes.removeClass("on"), board.userCells.each(function () { $(this).find("table.candidates td").empty() }), Object.keys(options.notes.possibilities).length && options.notes.render(), this.toggle = !1)
        }
    }
}, utility = {
    endcodeKey: "-ABCDEFGHI", alert: {
        alertObj: $("<div>").addClass("alert").append("<div>"), toggle: !1, timeout: null, value: null, say: function (a) {
            function b() { c.alertObj.find("div").text(a); clearTimeout(c.timeout); c.timeout = window.setTimeout(function () { c.hide() }, 1500) } if (a) {
                var c = this; this.toggle &&
                (c.alertObj.find("div").text(a), b()); this.toggle = !0; this.value = a; b(); this.render()
            }
        }, shout: function (a) { var b = this, c = $("<button/>").html("&times;"); c.bind("click", function () { b.hide() }); clearTimeout(this.timeout); this.alertObj.addClass("shout").find("div").text(a).append(c); this.render() }, render: function () { this.alertObj.removeClass("hide").appendTo(document.body) }, hide: function () {
            var a = this; this.toggle = !1; this.alertObj.addClass("hide"); setTimeout(function () { a.alertObj.remove().removeAttr("style") },
            500)
        }
    }, transformArray: function (a) { var b = [], c, d, e, f, h; for (d = 0; 3 > d; d++) for (e = 0; 3 > e; e++) { c = []; for (f = 0; 3 > f; f++) for (h = 0; 3 > h; h++) c.push(a[f + 3 * d][h + 3 * e]); b.push(c) } return b }, flatten: function (a) { return a.reduce(function (a, c) { return a.concat(c) }) }, isCharNum: function (a) { return 0 == 0 * a }, flattenBoard: function (a) { a = this.transformArray(a); a = this.flatten(a); for (var b = a.length; b--;) a[b] || (a[b] = 0); return a }, getDiffArrays: function (a, b) { var c = {}, d; for (d in a) b[d] != a[d] && (c[d] = a[d]); return c }, mergeEncode: function (a, b) {
        for (var c =
        a.length; c--;) a[c] != b[c] && (a[c] = utility.endcodeKey[b[c]])
    }, decodeStr: function (a) { if (81 > a.length) return !1; var b = {}, c; for (c in a) this.isCharNum(a[c]) || (b[c] = a.charCodeAt(c) - 64); $.isEmptyObject(b) && (b = !1); return b }, support: { storage: function () { try { return "localStorage" in window && null !== window.localStorage } catch (a) { return !1 } }() }, generateNumber: function (a, b) { a = a || 1; b = b || 9; return Math.random() * b + a >> 0 }, countEmtpyCells: function (a) { a = a || board.arr; a = utility.flatten(a); return a.length - a.filter(Number).length }
},
board = {
    mainBoard: null, level: "easy", $cellsArr: [], userCells: {}, arr: [], arr_original: [], movesHistory: [], spawnQueue: [], games: {}, boards: [], numberOfMoves: 0, numOfBoards: 0, boardsReadyTimeout: 0, domFragments: { userCell: '<div><div><span></span></div><button title="' + texts.clear_cell + '">&times;</button></div>' }, timerElm: $("<time>").attr("title", "Click to pasue/play"), init: function (a) {
        this.container = a; this.mainBoard = this.generateMainTable(); this.timer = new Timer(this.timerElm); this.timerElm.prependTo("#header");
        82 == window.location.search.length ? options.Import(window.location.search.split("?")[1], !0) : (this.games[this.level] && this.loadGameByLevel(this.level), this.spawnBoards(10)); this.bindUserEvents(); this.selectBox.init(); options.bind_buttons(); a.append(this.mainBoard)
    }, spawnBoards: function (a) { a = a || 2; this.numOfBoards += a; var b; for (b = this.populate() ; a--;) new board.CreateGame(b) }, generateMainTable: function () {
        var a = this, b, c; this.$cellsArr = []; b = this.makeTable(); b.find("td").each(function (b) {
            c = a.makeTable(b).addClass("box b" +
            b); a.$cellsArr.push(c.find("td").map(function () { return $(this) })); $(this).append(c)
        }); this.$cellsArr = utility.flatten(utility.transformArray(this.$cellsArr)); return b
    }, bindUserEvents: function () {
        var a = this, b, c, d, e, f, h; $(a.mainBoard).on("click", "td.user", a.cellClick).on("click", "td.user button", a.clearCell).on("mouseenter", "td.user", function (a) { b = $(a.currentTarget).data("pos") }).on("mouseleave", "td.user", function (a) { b = null }); $(document).bind("keyup", function (g) {
            h = g.which; c = 58 > h && 48 < h; d = 106 > h && 96 < h; e =
            46 == h || 110 == h; b && (a.selectBox.hide(), c || d ? (f = String.fromCharCode(96 <= h && 105 >= h ? h - 48 : h) | 0, options.notes.toggle ? a.changeNote(f, b) : a.changeCell(f, b)) : e && board.changeCell("", b), options.undoToggle())
        })
    }, selectBox: function () {
        function a(a) { var b = e.arr[a[0]][a[1]]; e.arr[a[0]][a[1]] = ""; e.selectBox.obj.find("td").each(function (b) { this.innerHTML = b + 1; this.className = e.checkNumber(b + 1, a) ? "" : "disabled" }); e.arr[a[0]][a[1]] = b } function b(a) { e.selectBox.obj.removeClass("show"); $(document).off("click.selectBox", d) } function c(a) {
            var c =
            a.target.innerHTML | 0; a = $(a.target).parents("td.user").data("pos"); options.notes.toggle ? e.changeNote(c, a) : e.changeCell(c, a); b()
        } function d(a) { a = $(a.target); if (a.hasClass("num_select") || a.parents("table").hasClass("num_select")) return !1; b() } var e; return {
            init: function () { e = board; this.obj = e.makeTable().addClass("num_select"); $(e.mainBoard).on("click", "table.num_select td:not(.disabled)", c) }, show: function (b, c) {
                a(c); $(b).children("div").append(e.selectBox.obj); setTimeout(function () {
                    e.selectBox.obj.toggleClass("show");
                    $(document).on("click.selectBox", d)
                }, 20)
            }, hide: b
        }
    }(), cellClick: function (a) { if (!$(a.target).closest("table").hasClass("num_select")) { var b = board; a = a.currentTarget; var c = $(a).data("pos"); b.selectBox.obj.is(":visible") && b.selectBox.hide(0); b.selectBox.show(a, c) } }, clearCell: function (a) { a.stopPropagation(); a = $(a.target).closest("td.user").data("pos"); board.changeCell("", a); options.undoToggle() }, changeCell: function (a, b) {
        var c, d; arguments.length ? this.movesHistory.push([b, this.arr[b[0]][b[1]]]) : (b = this.movesHistory.slice(-1)[0][0],
        a = this.movesHistory.slice(-1)[0][1]); c = this.arr[b[0]][b[1]]; this.arr[b[0]][b[1]] = a; d = this.userCells.filter(function () { return $(this).data("pos") == b }); a && d.find("> div span").text(a); a ? d.addClass("filled") : d.removeClass("filled"); options.undoToggle(); this.checkWin(c, a); options.notes.check(a, b); options.candidates.toggle && options.candidates.render()
    }, checkWin: function (a, b) {
        a ? b || board.numberOfMoves++ : b && board.numberOfMoves--; if (board.numberOfMoves) return !1; for (var c = utility.flattenBoard(this.arr).join(""),
        d = 1; 9 >= d; d++) if (9 > c.split(d).length - 1) return utility.alert.say(texts.loose), !1; board.endGame()
    }, changeNote: function (a, b) { var c, d; c = board.userCells.filter(function () { return $(this).data("pos") == b }); d = c.find("table.candidates td").eq(a - 1); c.removeClass("filled").find("> div span:first").text(""); board.arr[b[0]][b[1]] = ""; a = d.is(":empty") ? a : ""; d.html(a); options.notes.update(b, a) }, makeTable: function (a) {
        var b = $("<table>"), c, d, e, f; b.attr("cellpadding", 0).attr("cellspacing", 0); for (c = 0; 3 > c; c++) for (d = $("<tr>").appendTo(b),
        e = 0; 3 > e; e++) f = $("<td>"), a + 1 && f.data("pos", a + "" + (3 * c + e)), d.append(f); return b
    }, populate: function () {
        for (var a = 0, b = [4, 9], c = []; 9 > c.push([]) ;); for (var c = { arr: c }, d = [], e = 0; 9 > e; e++) {
            for (var f = 0; 9 > f; f++) { a++; b[1] = 9; d.length = 0; for (var h = utility.generateNumber() ; !this.checkNumber.apply(c, [h, [e, f], !0]) && b[1];) { a++; if (5E4 < a) return; -1 != d.indexOf(h) ? h = utility.generateNumber() : (d.push(h), h = utility.generateNumber(), b[1]--) } if (!b[1]) { b[0]--; c.arr[e].length = 0; e = b[0] ? e - 1 : e - 2; break } 9 == f && (b[0] = 4); c.arr[e][f] = h } b[0] ||
            (b[0] = 4)
        } return c.arr
    }, checkNumber: function (a, b, c) {
        var d = this, e; e = { square: function (b) { return -1 != d.arr[b].join("").indexOf(a) }, row: function (b) { if (0 == Math.ceil(b[0] % 3) && c) return !1; var e, g, k; e = Math.ceil(((b[0] | 0) + 1) / 3) - 1; b = Math.ceil(((b[1] | 0) + 1) / 3) - 1; for (g = 0; 3 > g; g++) for (k = 0; 3 > k; k++) if (a == d.arr[3 * e + g][3 * b + k]) return !0; return !1 }, col: function (b) { if (3 > b[0] && c) return !1; var e, g, k; e = Math.ceil((b[0] | 0) % 3); b = Math.ceil((b[1] | 0) % 3); for (g = 0; 3 > g; g++) for (k = 0; 3 > k; k++) if (a == d.arr[e + 3 * g][b + 3 * k]) return !0; return !1 } }; return -1 !=
        d.arr[b[0] | 0].join("").indexOf(a) || e.row(b) || e.col(b) ? !1 : !0
    }, removeItems: function (a) { a = a || { cells: 0 }; var b = a.arr || this.arr, c = []; do c[0] = utility.generateNumber() - 1, c[1] = utility.generateNumber() - 1; while ("" == b[c[0]][c[1]]); var d = [c, b[c[0]][c[1]], b[8 - c[0]][8 - c[1]]]; b[c[0]][c[1]] = ""; b[8 - c[0]][8 - c[1]] = ""; 1 < a.userCells && this.removeItems({ cells: --a.userCells, arr: b }); return d }, renderCellValues: function (a) { for (var b in a) this.$cellsArr[b].addClass("filled").find("span:first").html(a[b]) }, load: function (a) {
        a =
        a || {}; var b = a.filledCells || {}, c = this.generateMainTable(), d = c.find("table.box"), e = this, f, h, g, k, l, m, n, p = this.mainBoard.data("timeout") || null, q = 200; this.arr = a.arr ? $.extend(!0, [], a.arr) : this.arr; this.arr_original = $.extend(!0, [], this.arr); this.mainBoard.addClass("hideCells"); p && "placeholder" in document.createElement("input") ? (clearTimeout(p), p = null) : q = 0; p = setTimeout(function () {
            f = 0; for (h = e.arr.length; f < h; f++) for (k = d.eq(f), g = 0; g < h; g++) m = k.find("> tbody > tr > td").eq(g), l = board.arr[f][g], "" == l || b[f + "" + g] ?
            (m.addClass("user").html(board.domFragments.userCell), "" != l && m.addClass("filled").find("> div span:first").html(l)) : m.html("<span>" + board.arr[f][g] + "</span>").removeAttr("class"); e.mainBoard.contents().replaceWith(c.contents()); setTimeout(function () { e.mainBoard.removeClass("hideCells") }, 50); board.userCells = d.find("td.user"); board.userCells.each(function () { n = board.makeTable().addClass("candidates"); this.children[0].appendChild(n[0]) })
        }, q); this.mainBoard.data("timeout", p)
    }, loadGameByLevel: function (a) {
        if (a in
        this.games && this.games[a].length) { var b = utility.generateNumber(0, this.games[a].length); (a = this.games[a].splice(b, 1)[0]) && options.Import(a) } else utility.alert.say(texts.no_games_in_level); this.spawnBoards(4)
    }, endGame: function () {
        function a() { c += .15; d = utility.generateNumber(0, 80); var f = b.eq(d); f.css("backgroundColor", "rgba(190, 190, 190,.24)"); setTimeout(function () { f.removeAttr("style") }, 200); e = setTimeout(a, c | 0) } var b, c, d, e; this.timer.stop(); utility.alert.shout(texts.win); b = $("table.box > tbody > tr > td");
        c = 0; a(); $("#options > button").prop("disabled", !0); setTimeout(function () { clearTimeout(e); $("#options > button").prop("disabled", !1) }, 5E3)
    }, CreateGame: function () {
        var a = 0, b = function (b) { b && b.length ? (this.origArr = $.extend(!0, [], b), this.matrix_arr = [], this.arr = $.extend(!0, [], b), this.solutionCounter, this.triesCounter = 16, this.verifySolutionCounter = 0, this.state, a++, window.Worker && 20 < a ? board.spawnQueue.push(this) : this.init()) : console.warn("CreateGame: invalid or missing game array:", b) }; b.prototype = {
            init: function () {
                window.Worker &&
                (this.worker = new Worker("js/DLX.js"), this.worker.addEventListener("message", this.workerMessage.bind(this), !1)); board.removeItems({ cells: 8, arr: this.arr }); this.removeAndCheck()
            }, workerMessage: function (a) { this.solutionCounter = a.data; this.verifySolution() }, verifySolution: function () {
                this.verifySolutionCounter++; 2 == this.solutionCounter ? (this.triesCounter--, this.restoreState(this.arr), this.triesCounter ? this.removeAndCheck() : this.done()) : 81 < this.verifySolutionCounter ? (console.warn("verifySolution: too much recursion"),
                this.done()) : this.removeAndCheck()
            }, restoreState: function (a) { this.arr[this.state[0][0]][this.state[0][1]] = this.state[1]; this.arr[8 - this.state[0][0]][8 - this.state[0][1]] = this.state[2] }, done: function () { a--; board.initCreatedGame($.extend(!0, [], this.arr), $.extend(!0, [], this.origArr), this.worker) }
        }; b.prototype.removeAndCheck = window.Worker ? function () { this.state = board.removeItems({ arr: this.arr }); this.matrix_arr = utility.transformArray(this.arr); this.worker.postMessage(this.matrix_arr) } : function () {
            this.state =
            board.removeItems({ arr: this.arr }); this.matrix_arr = utility.transformArray(this.arr); this.solutionCounter = dlx.solve(this.matrix_arr); this.verifySolution()
        }; return b
    }(), initCreatedGame: function (a, b, c) {
        this.numOfBoards--; c.terminate(); board.spawnQueue.length && board.spawnQueue.pop().init(); 45 > utility.countEmtpyCells(a) ? (console.warn("generated board has too few moves. make a new game"), this.numOfBoards || this.boards.length || this.spawnBoards(2)) : (solver.init(a).step(0), this.boards.push([solver.grade, a]),
        a = this.addBoards(a, b), 1 == this.boards.length && 9 > this.arr.length && solver.score() == board.level && (this.preloadBoard(), this.solution = a))
    }, preloadBoard: function () { var a = this; if (this.boards.length) { var b = this.boards[0]; this.numberOfMoves = utility.countEmtpyCells(b[1]); this.load({ arr: b[1] }); solver.grade = b[0]; solver.setScore(); this.boards.splice(0, 1); this.timer.go(); 4 > this.numOfBoards && 4 > this.boards.length && this.spawnBoards(8) } else this.boardsReadyTimeout = setTimeout(function () { a.preloadBoard() }, 150) }, addBoards: function (a,
    b) { var c = solver.score(), d; d = this.getGameString(a, b); this.games[c] || (this.games[c] = []); this.games[c].push(d); return d }, getGameString: function (a, b) { a = utility.flattenBoard(a); b = utility.flattenBoard(b); utility.mergeEncode(a, b); return a.join("") }
}, solver = {
    init: function (a, b) { this.debug = b || 0; this.grade = this.steps = this.singles = 0; this.arr = a ? $.extend(!0, [], a) : $.extend(!0, [], board.arr); this.numberOfMoves = utility.countEmtpyCells(a); analyzer.evaluateTotal(solver); return this }, step: function (a) {
        var b = 0; switch (a) {
            case 0: analyzer.evaluatePosib();
                break; case 1: analyzer.singleInBox(); break; case 2: analyzer.singleInRowCol(); break; case 3: b += analyzer.nakedPairsTriples(); break; case 4: b += analyzer.hiddenPairsTriples(); break; default: return !1
        } this.makeSingleMoves(); this.grade += 10; 2 == a && (this.grade += 10 * this.singles); 3 < a && b && (this.grade += 10 * b); if (4 == a && !b || 0 == this.numberOfMoves) return this.grade += 30 * this.numberOfMoves, !1; 150 < this.steps || (a = 0 < this.singles || 0 < b ? 0 : a + 1, this.step(a))
    }, makeSingleMoves: function () {
        this.steps++; this.singles = 0; var a = analyzer.possibilities,
        b; for (b in a) a.hasOwnProperty(b) && 1 === a[b].length && (this.singles++, this.numberOfMoves--, this.advance(b, a[b][0]), delete a[b])
    }, advance: function (a, b) { this.arr[a[0]][a[1]] = b; this.debug && board.mainBoard.find(".box").eq(a[0]).find("> tbody > tr > td").eq(a[1]).html("<span>" + b + "</span") }, score: function (a) { a = "very easy"; 1400 < this.grade ? a = "extreme" : 1200 < this.grade ? a = "very tough" : 900 < this.grade ? a = "tough" : 350 < this.grade ? a = "medium" : 150 < this.grade && (a = "easy"); return a }, setScore: function (a) {
        a = a || this.score();
        $("div.output:first > span").text(a).attr({ "data-value": solver.grade, title: "more or less" }).hide().show(0)
    }
}, analyzer = {
    possibilities: {}, evaluateTotal: function (a) { var b = this.possibilities = {}, c, d, e; for (c = 0; 9 > c; c++) for (d = 0; 9 > d; d++) if ("" == a.arr[c][d]) for (b[c + "" + d] = [], e = 1; 10 > e; e++) board.checkNumber.apply(a, [e, [c, d]]) && b[c + "" + d].push(e); return b }, evaluatePosib: function (a) {
        var b = this.possibilities, c, d, e; a = a || solver; for (c in b) for (d = b[c].length; d--;) !1 === board.checkNumber.apply(a, [b[c][d], [c[0] | 0, c[1] | 0]]) &&
        (e = b[c].indexOf(b[c][d]), b[c].splice(e, 1)); return b
    }, singleInBox: function () { var a = this.possibilities, b = Object.keys(a)[0][0], c = this.boxPosib = [{}], d; for (d in a) d[0] != b && (b = d[0], c.push({})), c[c.length - 1][d] = a[d]; for (a = c.length; a--;) this.hiddenSingles(c[a]) }, singleInRowCol: function () {
        for (var a = this.possibilities, b = this.colPosib = [], c = this.rowPosib = [], d = 9; d--;) b.push({}), c.push({}); for (var e in a) d = 3 * (Math.ceil(((e[0] | 0) + 1) / 3) - 1) + (Math.ceil(((e[1] | 0) + 1) / 3) - 1), b[3 * Math.ceil(e[0] % 3) + Math.ceil(e[1] % 3)][e] =
        a[e], c[d][e] = a[e]; for (d = 0; 9 > d; d++) this.hiddenSingles(b[d]), this.hiddenSingles(c[d])
    }, hiddenSingles: function (a) { var b = "", c; for (c in a) b += a[c].join(""); for (c in a) for (var d = a[c].length; d--;) { var e = a[c][d]; if (b.indexOf(e) == b.lastIndexOf(e)) { this.possibilities[c] = [e]; break } } }, nakedPairsTriples: function () {
        for (var a = 0, b = 9; b--;) a += this.findNakedPairs(this.rowPosib[b]) + this.findNakedPairs(this.colPosib[b]) + this.findNakedPairs(this.boxPosib[b]) + this.findNakedTriples(this.rowPosib[b]) + this.findNakedTriples(this.colPosib[b]) +
        this.findNakedTriples(this.boxPosib[b]); return a
    }, hiddenPairsTriples: function () { for (var a = 0, b = 9; b--;) a += this.findHiddenPairsTriples(this.rowPosib[b]) + this.findHiddenPairsTriples(this.colPosib[b]) + this.findHiddenPairsTriples(this.boxPosib[b]); return a }, findNakedPairs: function (a) {
        if ("object" != typeof a || 3 > Object.keys(a).length) return 0; var b = [], c = 0, d; for (d in a) 2 == a[d].length && b.push(d); d = b.length; for (var e = 0; e < d; e++) for (var f = e + 1; f < d; f++) if (2 == a[b[e]].length && a[b[e]].join("") == a[b[f]].join("")) {
            var h =
            a[b[e]].join(""), g; for (g in a) if (g != b[e] && g != b[f]) for (var k = a[g].length; k--;) -1 != h.indexOf(a[g][k]) && (c++, this.possibilities[g].splice(k, 1))
        } return c
    }, findNakedTriples: function (a) {
        if ("object" != typeof a || 4 > Object.keys(a).length) return 0; var b = [], c = 0, d; for (d in a) 4 > a[d].length && b.push(d); d = []; for (var e = b.length, f = 0; f < e; f++) for (var h = f + 1; h < e; h++) for (var g = h + 1; g < e; g++) {
            d.push(b[f], b[h], b[g]); for (var k = "", l = d.length; l--;) for (var m = a[d[l]].length; m--;) { var n = a[d[l]][m]; -1 == k.indexOf(n) && (k += n) } if (3 == k.length) for (l in a) if (-1 ==
            d.indexOf(l)) for (m = a[l].length; m--;) -1 != k.indexOf(a[l][m]) && (c++, this.possibilities[l].splice(m, 1)); d.length = 0
        } return c
    }, findHiddenPairsTriples: function (a) {
        function b(a, b) { for (var e = a.length; e--;) c.possibilities[d[a[e]]] = $.extend(!0, [], b) } var c = this, d = [], e = "", f = [], h = 0, g; for (g in a) d.push(g), e += a[g].join(""); for (g = 0; g < d.length; g++) for (var k = g + 1; k < d.length; k++) {
            for (var l = f.length = 0; l < a[d[g]].length; l++) {
                var m = a[d[g]][l]; 4 < a[d[g]].length + a[d[k]].length && 3 == e.split(m).length && -1 != a[d[k]].indexOf(m) &&
                f.push(m); if (2 == f.length) { h++; b([g, k], f); break }
            } for (var n = k + 1; n < d.length; n++) for (l = f.length = 0; l < a[d[g]].length; l++) if (m = a[d[g]][l], 9 < a[d[g]].length + a[d[k]].length + a[d[n]].length && 4 == e.split(m).length && -1 != a[d[k]].indexOf(m) && -1 != a[d[n]].indexOf(m) && f.push(m), 3 == f.length) { h++; b([g, k, n], f); break }
        } return h
    }
}, Timer = function (a) { this.el = a; this.hour = this.min = this.sec = 0; this.focus = this.refresh = null; this.paused = !1; a.html("00:00"); this.events(); return this };
Timer.prototype = {
    set: function (a) { a = a.split(":"); this.hour = a[0]; this.min = a[1]; this.sec = a[2]; return this }, bindFocus: function () { var a = this; clearInterval(this.focus); document.hasFocus && (a.focus = setInterval(function () { document.hasFocus() ? a.paused && (window.clearInterval(this.refresh), a.go()) : a.stop() }, 200)) }, events: function () {
        var a = this, b = $("<h2>").addClass("pauseGame").text(texts.pause), c = !0; this.el.on("click", function (d) {
            c && (c = !1, !1 === a.paused ? (a.stop(), window.clearInterval(a.focus), $("#options > button").prop("disabled",
            !0), board.mainBoard.fadeOut(400, function () { b.css({ letterSpacing: "25px", opacity: 0 }); $(this).after(b).detach(); b.parent().addClass("paused"); b.animate({ opacity: 1 }, { queue: !1, duration: 400 }).animate({ letterSpacing: "-4px" }, 700, "easeOutBack", function () { c = !0 }) }), a.el.addClass("pause")) : ($("#options > button").prop("disabled", !1), options.undoToggle(), b.animate({ opacity: 0, letterSpacing: "25px" }, 600, "easeInBack", function () {
                $(this).parent().removeClass("paused").end().remove(); board.container.prepend(board.mainBoard).removeAttr("style");
                board.mainBoard.fadeIn(400); a.go(); c = !0
            }), this.className = ""))
        })
    }, restart: function (a) { this.sec = this.min = this.hour = 0; this.el.text("00:00"); this.stop().go() }, go: function (a) { function b() { c.sec++; 60 == c.sec && (c.sec = 0, c.min++); 10 > c.sec && (c.sec = "0" + c.sec); 60 == c.min && (c.min = 0, c.hour++); c.el.html((0 < c.hour ? (9 >= c.hour ? "0" + c.hour : c.hour) + ":" : "") + (9 >= c.min ? "0" + c.min : c.min) + ":" + c.sec) } var c = this; this.paused = !1; a && b(); window.clearInterval(this.refresh); c.refresh = window.setInterval(b, 1E3); return this }, stop: function () {
        this.paused =
        !0; window.clearInterval(this.refresh); return this
    }
};