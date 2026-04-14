--
-- PostgreSQL database dump
--

\restrict 6PhvGfH7DbtW4KEpCzCSkav7X8UyxYpZ4vSuCxBWZSLl6s5QUywnyEHMQXJE9h6

-- Dumped from database version 18.3
-- Dumped by pg_dump version 18.3

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: fcc_leagues; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.fcc_leagues (
    teamid integer,
    leagueid integer
);


ALTER TABLE public.fcc_leagues OWNER TO postgres;

--
-- Data for Name: fcc_leagues; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.fcc_leagues (teamid, leagueid) FROM stdin;
0	0
1	0
2	0
3	0
4	0
5	0
6	0
7	0
8	0
9	0
10	0
11	0
12	0
13	0
14	0
15	0
16	0
17	0
18	0
19	0
20	0
21	0
22	0
23	0
24	0
25	0
26	0
27	0
28	0
29	0
30	0
31	0
32	1
33	1
34	1
35	1
36	1
37	1
38	1
39	1
40	1
41	1
42	1
43	1
44	1
45	1
46	1
47	1
48	1
49	1
50	1
51	1
52	1
53	1
54	1
55	1
56	1
57	1
58	1
59	1
60	1
61	1
62	2
63	2
64	2
65	2
66	2
67	2
68	2
69	2
70	2
71	2
72	2
73	2
74	3
75	3
76	3
77	3
78	3
79	3
80	3
81	3
82	3
83	3
84	3
85	3
86	3
87	3
88	4
89	4
90	4
91	4
92	4
93	4
94	4
95	4
96	4
97	4
98	4
99	4
100	4
101	4
102	4
103	5
104	5
105	5
106	5
107	5
108	5
109	5
110	5
111	5
112	5
113	5
114	5
115	5
116	5
117	6
118	6
119	6
120	6
121	6
122	6
123	6
124	6
125	6
126	6
127	6
128	6
129	7
130	7
131	7
132	7
133	7
134	7
135	7
136	7
137	7
138	7
139	7
140	7
141	7
142	7
143	7
144	7
145	7
146	7
147	7
148	7
149	7
150	8
151	8
152	8
153	8
154	8
155	8
156	8
157	8
158	8
159	8
160	8
161	8
162	8
163	8
164	8
165	8
166	8
167	8
168	8
169	8
170	9
171	9
172	9
173	9
174	9
175	9
176	9
177	9
178	9
179	9
180	9
181	9
182	9
183	9
184	9
185	9
186	9
187	9
188	10
189	10
190	10
191	10
192	10
193	10
194	10
195	10
196	10
197	10
198	10
199	10
200	10
201	10
202	10
203	10
204	10
205	10
206	10
207	10
208	10
209	10
210	11
211	11
\.


--
-- PostgreSQL database dump complete
--

\unrestrict 6PhvGfH7DbtW4KEpCzCSkav7X8UyxYpZ4vSuCxBWZSLl6s5QUywnyEHMQXJE9h6

