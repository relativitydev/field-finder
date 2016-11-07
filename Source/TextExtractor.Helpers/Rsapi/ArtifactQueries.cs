using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using kCura.Relativity.Client;
using kCura.Relativity.Client.DTOs;
using Relativity.API;

namespace TextExtractor.Helpers.Rsapi
{
	public class ArtifactQueries : IArtifactQueries
	{
		public static Boolean DoesUserHaveAccessToArtifact(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Guid guid, String artifactTypeName)
		{
			var result = DoesUserHaveAccessToRdoByType(svcMgr, identity, workspaceArtifactId, guid, artifactTypeName);
			var hasAccess = result.Success;

			return hasAccess;
		}

		private static Response<Boolean> DoesUserHaveAccessToRdoByType(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Guid guid, String artifactTypeName)
		{
			ResultSet<RDO> results;
			using (var client = svcMgr.CreateProxy<IRSAPIClient>(identity))
			{
				client.APIOptions.WorkspaceID = workspaceArtifactId;
				var relApp = new RDO(guid) { ArtifactTypeName = artifactTypeName };

				results = client.Repositories.RDO.Read(relApp);
			}

			var res = new Response<Boolean> { Results = results.Success, Success = results.Success, Message = MessageFormatter.FormatMessage(results.Results.Select(x => x.Message).ToList(), results.Message, results.Success) };

			return res;
		}

		public void UpdateRdoStringFieldValue(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Guid objectGuid, Guid fieldGuid, Int32 objectArtifactId, String fieldValue)
		{
			using (var client = svcMgr.CreateProxy<IRSAPIClient>(identity))
			{
				client.APIOptions.WorkspaceID = workspaceArtifactId;

				try
				{
					var rdo = new RDO(objectArtifactId);
					rdo.ArtifactTypeGuids.Add(objectGuid);
					rdo.Fields.Add(new FieldValue(fieldGuid, fieldValue));

					var result = client.Repositories.RDO.Update(rdo);
					if (!result.Success)
					{
						var messageList = new StringBuilder();
						messageList.AppendLine(result.Message);
						result.Results.ToList().ForEach(w => messageList.AppendLine(w.Message));
						throw new Exception(messageList.ToString());
					}
				}
				catch (Exception ex)
				{
					throw new Exception("Error encountered, ", ex);
				}
			}
		}

		public String GetExtractorSetStatus(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 extractorSetArtifactId)
		{
			var errorContext = String.Format("An error occured when querying for Extractor Set status. [WorkspaceArtifactId: {0}, ExtractorSetArtifactId: {1}]", workspaceArtifactId, extractorSetArtifactId);

			try
			{
				using (var proxy = svcMgr.CreateProxy<IRSAPIClient>(identity))
				{
					proxy.APIOptions.WorkspaceID = workspaceArtifactId;
					String status;
					try
					{
						var jobRdo = proxy.Repositories.RDO.ReadSingle(extractorSetArtifactId);
						status = jobRdo.Fields.Get(Constant.Guids.Fields.ExtractorSet.Status).ValueAsFixedLengthText;
					}
					catch (Exception ex)
					{
						throw new CustomExceptions.TextExtractorException(errorContext + ". ReadSingle failed.", ex);
					}
					return status;
				}
			}
			catch (Exception ex)
			{
				throw new CustomExceptions.TextExtractorException(errorContext, ex);
			}
		}

		public String GetExtractorSetDetails(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 extractorSetArtifactId)
		{
			var errorContext = String.Format("An error occured when querying for Extractor Set details. [WorkspaceArtifactId: {0}, ExtractorSetArtifactId: {1}]", workspaceArtifactId, extractorSetArtifactId);

			try
			{
				using (var proxy = svcMgr.CreateProxy<IRSAPIClient>(identity))
				{
					proxy.APIOptions.WorkspaceID = workspaceArtifactId;
					RDO jobRdo;
					try
					{
						jobRdo = proxy.Repositories.RDO.ReadSingle(extractorSetArtifactId);
					}
					catch (Exception ex)
					{
						throw new CustomExceptions.TextExtractorException(errorContext + ". ReadSingle failed.", ex);
					}
					var details = jobRdo.Fields.Get(Constant.Guids.Fields.ExtractorSet.Details).ValueAsLongText ?? string.Empty;
					return details;
				}
			}
			catch (Exception ex)
			{
				throw new CustomExceptions.TextExtractorException(errorContext, ex);
			}
		}

		public void UpdateTotalExpectedUpdates(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 extractorSetArtifactId, Int32 totalExpectedUpdates)
		{
			using (var client = svcMgr.CreateProxy<IRSAPIClient>(identity))
			{
				client.APIOptions.WorkspaceID = workspaceArtifactId;

				var jobRdo = new RDO(extractorSetArtifactId)
				{
					ArtifactTypeGuids = new List<Guid> { Constant.Guids.ObjectType.ExtractorSet },
					Fields = new List<FieldValue> { new FieldValue(Constant.Guids.Fields.ExtractorSet.TotalExpectedUpdates, totalExpectedUpdates) }
				};

				client.Repositories.RDO.UpdateSingle(jobRdo);
			}
		}

		public void UpdateNumberOfUpdatesWithValues(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 extractorSetArtifactId, Int32 numberOfUpdatesWithValues)
		{
			using (var client = svcMgr.CreateProxy<IRSAPIClient>(identity))
			{
				client.APIOptions.WorkspaceID = workspaceArtifactId;

				var jobRdo = new RDO(extractorSetArtifactId)
				{
					ArtifactTypeGuids = new List<Guid> { Constant.Guids.ObjectType.ExtractorSet },
					Fields = new List<FieldValue> { new FieldValue(Constant.Guids.Fields.ExtractorSet.NumberOfUpdatesWithValues, numberOfUpdatesWithValues) }
				};

				client.Repositories.RDO.UpdateSingle(jobRdo);
			}
		}

		public void UpdateExtractorSetStatus(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 extractorSetArtifactId, String status)
		{
			using (var client = svcMgr.CreateProxy<IRSAPIClient>(identity))
			{
				client.APIOptions.WorkspaceID = workspaceArtifactId;
				var jobRdo = new RDO(extractorSetArtifactId)
				{
					ArtifactTypeGuids = new List<Guid> { Constant.Guids.ObjectType.ExtractorSet },
					Fields = new List<FieldValue> { new FieldValue(Constant.Guids.Fields.ExtractorSet.Status, status) }
				};

				try
				{
					client.Repositories.RDO.UpdateSingle(jobRdo);
				}
				catch (Exception ex)
				{
					throw new CustomExceptions.TextExtractorException("An error occurred when updating Extractor Set Status field.", ex);
				}
			}
		}

		public void UpdateExtractorSetDetails(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 extractorSetArtifactId, String details)
		{
			using (var client = svcMgr.CreateProxy<IRSAPIClient>(identity))
			{
				client.APIOptions.WorkspaceID = workspaceArtifactId;
				var jobRdo = new RDO(extractorSetArtifactId)
				{
					ArtifactTypeGuids = new List<Guid> { Constant.Guids.ObjectType.ExtractorSet },
					Fields = new List<FieldValue> { new FieldValue(Constant.Guids.Fields.ExtractorSet.Details, details) }
				};

				try
				{
					client.Repositories.RDO.UpdateSingle(jobRdo);
				}
				catch (Exception ex)
				{
					throw new CustomExceptions.TextExtractorException("An error occurred when updating Extractor Set Details field.", ex);
				}
			}
		}

		public RDO GetExtractorRegularExpressionRdo(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 fieldArtifactId)
		{
			var errorContext = String.Format("An error occured when querying for Regular Expression RDO. [WorkspaceArtifactId: {0}, FieldArtifactId: {1}]", workspaceArtifactId, fieldArtifactId);

			try
			{
				using (var proxy = svcMgr.CreateProxy<IRSAPIClient>(identity))
				{
					proxy.APIOptions.WorkspaceID = workspaceArtifactId;
					RDO fieldRdo;
					try
					{
						fieldRdo = proxy.Repositories.RDO.ReadSingle(fieldArtifactId);
					}
					catch (Exception ex)
					{
						throw new CustomExceptions.TextExtractorException(errorContext + ". ReadSingle failed.", ex);
					}
					return fieldRdo;
				}
			}
			catch (Exception ex)
			{
				throw new CustomExceptions.TextExtractorException(errorContext, ex);
			}
		}

		public RDO GetExtractorTargetTextRdo(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 fieldArtifactId)
		{
			var errorContext = String.Format("An error occured when querying for Target Text RDO. [WorkspaceArtifactId: {0}, FieldArtifactId: {1}]", workspaceArtifactId, fieldArtifactId);

			try
			{
				using (var proxy = svcMgr.CreateProxy<IRSAPIClient>(identity))
				{
					proxy.APIOptions.WorkspaceID = workspaceArtifactId;
					RDO fieldRdo;
					try
					{
						fieldRdo = proxy.Repositories.RDO.ReadSingle(fieldArtifactId);
					}
					catch (Exception ex)
					{
						throw new CustomExceptions.TextExtractorException(errorContext + ". ReadSingle failed.", ex);
					}
					return fieldRdo;
				}
			}
			catch (Exception ex)
			{
				throw new CustomExceptions.TextExtractorException(errorContext, ex);
			}
		}

		public RDO GetExtractorProfileRdo(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 extractorProfileArtifactId)
		{
			var errorContext = String.Format("An error occured when querying for Extractor Profile RDO. [WorkspaceArtifactId: {0}, ExtractorProfileArtifactId: {1}]", workspaceArtifactId, extractorProfileArtifactId);

			try
			{
				using (var proxy = svcMgr.CreateProxy<IRSAPIClient>(identity))
				{
					proxy.APIOptions.WorkspaceID = workspaceArtifactId;
					RDO templateRdo;
					try
					{
						templateRdo = proxy.Repositories.RDO.ReadSingle(extractorProfileArtifactId);
					}
					catch (Exception ex)
					{
						throw new CustomExceptions.TextExtractorException(errorContext + ". ReadSingle failed.", ex);
					}
					return templateRdo;
				}
			}
			catch (Exception ex)
			{
				throw new CustomExceptions.TextExtractorException(errorContext, ex);
			}
		}

		public RDO GetExtractorSetRdo(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 extractorSetArtifactId)
		{
			var errorContext = String.Format("An error occured when querying for Extractor Set RDO. [WorkspaceArtifactId: {0}, ExtractorSetArtifactId: {1}]", workspaceArtifactId, extractorSetArtifactId);

			try
			{
				using (var proxy = svcMgr.CreateProxy<IRSAPIClient>(identity))
				{
					proxy.APIOptions.WorkspaceID = workspaceArtifactId;
					RDO jobRdo;
					try
					{
						jobRdo = proxy.Repositories.RDO.ReadSingle(extractorSetArtifactId);
					}
					catch (Exception ex)
					{
						throw new CustomExceptions.TextExtractorException(errorContext + ". ReadSingle failed.", ex);
					}
					return jobRdo;
				}
			}
			catch (Exception ex)
			{
				throw new CustomExceptions.TextExtractorException(errorContext, ex);
			}
		}

		public QueryResultSet<Document> GetFirstBatchOfDocuments(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 batchSize, Query<Document> query, Int32 workspaceArtifactId)
		{
			QueryResultSet<Document> results;

			using (var client = svcMgr.CreateProxy<IRSAPIClient>(identity))
			{
				client.APIOptions.WorkspaceID = workspaceArtifactId;

				results = client.Repositories.Document.Query(query, batchSize);
			}
			return results;
		}

		public QueryResultSet<Document> GetSubsequentBatchOfDocuments(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 batchSize, Int32 startIndex, String token, Query<Document> query, Int32 workspaceArtifactId)
		{
			QueryResultSet<Document> results;

			using (var client = svcMgr.CreateProxy<IRSAPIClient>(identity))
			{
				client.APIOptions.WorkspaceID = workspaceArtifactId;

				results = client.Repositories.Document.QuerySubset(token, startIndex, batchSize);
			}
			return results;
		}

		public String GetDocumentTextFieldValue(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 documentArtifactId, Int32 fieldArtifactId)
		{
			var errorContext = String.Format("An error occured when querying for Document text field value. [WorkspaceArtifactId: {0}, DocumentArtifactId: {1}, FieldArtifactId: {2}]", workspaceArtifactId, documentArtifactId, fieldArtifactId);
			String retVal;

			try
			{
				using (var proxy = svcMgr.CreateProxy<IRSAPIClient>(identity))
				{
					proxy.APIOptions.WorkspaceID = workspaceArtifactId;

					var documentDto = new Document(documentArtifactId) { Fields = new List<FieldValue> { new FieldValue(fieldArtifactId) } };
					var resultSet = proxy.Repositories.Document.Read(documentDto); //we are using Read instead of ReadSingle since ReadSingle will get all the fields and to avoid any issues with the fields which we are not part of this application and cause any trouble due to size limitations

					if (resultSet.Success)
					{
						if (resultSet.Results != null && resultSet.Results.Count == 1)
						{
							var firstOrDefault = resultSet.Results.FirstOrDefault();
							if (firstOrDefault != null)
							{
								var documentArtifact = firstOrDefault.Artifact;
								var fieldValue = documentArtifact.Fields.Get(fieldArtifactId).Value;
								retVal = fieldValue != null ? fieldValue.ToString() : null;
							}
							else
							{
								//error
								throw new CustomExceptions.TextExtractorException(errorContext);
							}
						}
						else
						{
							//error
							throw new CustomExceptions.TextExtractorException(errorContext);
						}
					}
					else
					{
						//error
						throw new CustomExceptions.TextExtractorException(string.Format(errorContext + ". Error Message: {0}.", resultSet.Message));
					}
				}
			}
			catch (Exception ex)
			{
				throw new CustomExceptions.TextExtractorException(errorContext + ". Document Read failed.", ex);
			}

			return retVal;
		}

		public void UpdateDocumentTextFieldValue(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 documentArtifactId, Int32 fieldArtifactId, String fieldValue)
		{
			var errorContext = String.Format("An error occured when updating Document text field value. [WorkspaceArtifactId: {0}, DocumentArtifactId: {1}, FieldArtifactId: {2}]", workspaceArtifactId, documentArtifactId, fieldArtifactId);

			using (var proxy = svcMgr.CreateProxy<IRSAPIClient>(identity))
			{
				proxy.APIOptions.WorkspaceID = workspaceArtifactId;
				var documentDto = new Document(documentArtifactId);
				documentDto.Fields.Add(new FieldValue(fieldArtifactId) { Value = fieldValue });

				try
				{
					proxy.Repositories.Document.UpdateSingle(documentDto);
				}
				catch (Exception ex)
				{
					throw new CustomExceptions.TextExtractorException(errorContext, ex);
				}
			}
		}

		public String GetDocumentLongTextFieldValue(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 documentArtifactId, Guid fieldGuid)
		{
			var errorContext = String.Format("An error occured when updating Document TextExtractorDetails field value. [WorkspaceArtifactId: {0}, DocumentArtifactId: {1}]", workspaceArtifactId, documentArtifactId);
			String retVal;

			try
			{
				using (var proxy = svcMgr.CreateProxy<IRSAPIClient>(identity))
				{
					proxy.APIOptions.WorkspaceID = workspaceArtifactId;

					Document documentDto = new Document(documentArtifactId) { Fields = new List<FieldValue> { new FieldValue(fieldGuid) } };
					var resultSet = proxy.Repositories.Document.Read(documentDto); //we are using Read instead of ReadSingle since ReadSingle will get all the fields and to avoid any issues with the fields which we are not part of this application and cause any trouble due to size limitations

					if (resultSet.Success)
					{
						if (resultSet.Results != null && resultSet.Results.Count == 1)
						{
							var firstOrDefault = resultSet.Results.FirstOrDefault();
							if (firstOrDefault != null)
							{
								var documentArtifact = firstOrDefault.Artifact;
								var fieldValue = documentArtifact.Fields.Get(fieldGuid).Value;
								retVal = fieldValue != null ? fieldValue.ToString() : null;
							}
							else
							{
								//error
								throw new CustomExceptions.TextExtractorException(errorContext);
							}
						}
						else
						{
							//error
							throw new CustomExceptions.TextExtractorException(errorContext);
						}
					}
					else
					{
						//error
						throw new CustomExceptions.TextExtractorException(string.Format(errorContext + ". Error Message: {0}.", resultSet.Message));
					}
				}
			}
			catch (Exception ex)
			{
				throw new CustomExceptions.TextExtractorException(errorContext + ". Document Read failed.", ex);
			}

			return retVal;
		}

		public void UpdateDocumentLongTextFieldValue(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 documentArtifactId, Guid documentFieldGuid, String fieldValue)
		{
			var errorContext = String.Format("An error occured when updating Document TextExtractorDetails field value. [WorkspaceArtifactId: {0}, DocumentArtifactId: {1}]", workspaceArtifactId, documentArtifactId);

			try
			{
				using (var proxy = svcMgr.CreateProxy<IRSAPIClient>(identity))
				{
					proxy.APIOptions.WorkspaceID = workspaceArtifactId;
					var documentDto = new Document(documentArtifactId);
					documentDto.Fields.Add(new FieldValue(documentFieldGuid) { Value = fieldValue });

					try
					{
						proxy.Repositories.Document.UpdateSingle(documentDto);
					}
					catch (Exception ex)
					{
						throw new CustomExceptions.TextExtractorException(errorContext, ex);
					}
				}
			}
			catch (Exception ex)
			{
				throw new CustomExceptions.TextExtractorException(errorContext, ex);
			}
		}

		public void AppendToDocumentLongTextFieldValue(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 documentArtifactId, Guid documentFieldGuid, String fieldValue)
		{
			var currentFieldValue = GetDocumentLongTextFieldValue(svcMgr, identity, workspaceArtifactId, documentArtifactId, documentFieldGuid);
			String newFieldValue = String.Format("{0}{1}", string.IsNullOrEmpty(currentFieldValue) ? currentFieldValue : currentFieldValue + ". ", fieldValue);
			UpdateDocumentLongTextFieldValue(svcMgr, identity, workspaceArtifactId, documentArtifactId, documentFieldGuid, newFieldValue);
		}

		public void CreateExtractorSetHistoryRecord(IServicesMgr svcMgr, ExecutionIdentity identity, String name, Int32 workspaceArtifactId, Int32? extractorSetArtifactId, Int32? documentArtifactId, Int32? destinationFieldArtifactId, String status, String details, String targetName, String startMarker, String stopMarker, String markerType)
		{
			var errorContext = String.Format("An error occured when creating Extractor Set History Record. [WorkspaceArtifactId: {0}, DocumentArtifactId: {1}, TextExtractorSetArtifactId: {2}]", workspaceArtifactId, documentArtifactId, extractorSetArtifactId);

			try
			{
				using (var proxy = svcMgr.CreateProxy<IRSAPIClient>(identity))
				{
					proxy.APIOptions.WorkspaceID = workspaceArtifactId;

					var jobHistoryDto = new RDO();
					jobHistoryDto.ArtifactTypeGuids.Add(Constant.Guids.ObjectType.ExtractorSetHistory);
					jobHistoryDto.Fields.Add(new FieldValue(Constant.Guids.Fields.ExtractorSetHistory.Name) { Value = name });
					jobHistoryDto.Fields.Add(new FieldValue(Constant.Guids.Fields.ExtractorSetHistory.DocumentIdentifier) { Value = GetDocumentIdentifierValue(svcMgr, identity, workspaceArtifactId, Convert.ToInt32(documentArtifactId)) });
					jobHistoryDto.Fields.Add(new FieldValue(Constant.Guids.Fields.ExtractorSetHistory.DestinationField) { Value = GetFieldNameForArtifactId(svcMgr, identity, workspaceArtifactId, Convert.ToInt32(destinationFieldArtifactId)) });
					jobHistoryDto.Fields.Add(new FieldValue(Constant.Guids.Fields.ExtractorSetHistory.Status) { Value = status });
					jobHistoryDto.Fields.Add(new FieldValue(Constant.Guids.Fields.ExtractorSetHistory.Details) { Value = details });
					jobHistoryDto.Fields.Add(new FieldValue(Constant.Guids.Fields.ExtractorSetHistory.TargetName) { Value = targetName });
					jobHistoryDto.Fields.Add(new FieldValue(Constant.Guids.Fields.ExtractorSetHistory.MarkerType) { Value = markerType });
					jobHistoryDto.Fields.Add(new FieldValue(Constant.Guids.Fields.ExtractorSetHistory.StartMarker) { Value = startMarker });
					jobHistoryDto.Fields.Add(new FieldValue(Constant.Guids.Fields.ExtractorSetHistory.StopMarker) { Value = stopMarker });
					var jobRdo = extractorSetArtifactId != null ? new RDO(Convert.ToInt32(extractorSetArtifactId)) : null;
					jobHistoryDto.Fields.Add(new FieldValue(Constant.Guids.Fields.ExtractorSetHistory.ExtractorSet) { Value = jobRdo });

					try
					{
						proxy.Repositories.RDO.CreateSingle(jobHistoryDto);
					}
					catch (Exception ex)
					{
						throw new CustomExceptions.TextExtractorException(errorContext, ex);
					}
				}
			}
			catch (Exception ex)
			{
				throw new CustomExceptions.TextExtractorException(errorContext, ex);
			}
		}

		public void CreateExtractorSetHistoryRecord(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32? extractorSetArtifactId, Int32? documentArtifactId, String status, String details)
		{
			var errorContext = String.Format("An error occured when creating Extractor Set History Record. [WorkspaceArtifactId: {0}, DocumentArtifactId: {1}, TextExtractorSetArtifactId: {2}]", workspaceArtifactId, documentArtifactId, extractorSetArtifactId);

			try
			{
				using (var proxy = svcMgr.CreateProxy<IRSAPIClient>(identity))
				{
					proxy.APIOptions.WorkspaceID = workspaceArtifactId;

					var jobHistoryDto = new RDO();
					jobHistoryDto.ArtifactTypeGuids.Add(Constant.Guids.ObjectType.ExtractorSetHistory);
					jobHistoryDto.Fields.Add(new FieldValue(Constant.Guids.Fields.ExtractorSetHistory.Name) { Value = Guid.NewGuid().ToString() });
					jobHistoryDto.Fields.Add(new FieldValue(Constant.Guids.Fields.ExtractorSetHistory.DocumentIdentifier) { Value = GetDocumentIdentifierValue(svcMgr, identity, workspaceArtifactId, Convert.ToInt32(documentArtifactId)) });
					jobHistoryDto.Fields.Add(new FieldValue(Constant.Guids.Fields.ExtractorSetHistory.DestinationField) { Value = String.Empty });
					jobHistoryDto.Fields.Add(new FieldValue(Constant.Guids.Fields.ExtractorSetHistory.Status) { Value = status });
					jobHistoryDto.Fields.Add(new FieldValue(Constant.Guids.Fields.ExtractorSetHistory.Details) { Value = details });
					jobHistoryDto.Fields.Add(new FieldValue(Constant.Guids.Fields.ExtractorSetHistory.TargetName) { Value = String.Empty });
					jobHistoryDto.Fields.Add(new FieldValue(Constant.Guids.Fields.ExtractorSetHistory.MarkerType) { Value = String.Empty });
					jobHistoryDto.Fields.Add(new FieldValue(Constant.Guids.Fields.ExtractorSetHistory.StartMarker) { Value = String.Empty });
					jobHistoryDto.Fields.Add(new FieldValue(Constant.Guids.Fields.ExtractorSetHistory.StopMarker) { Value = String.Empty });
					var jobRdo = extractorSetArtifactId != null ? new RDO(Convert.ToInt32(extractorSetArtifactId)) : null;
					jobHistoryDto.Fields.Add(new FieldValue(Constant.Guids.Fields.ExtractorSetHistory.ExtractorSet) { Value = jobRdo });

					try
					{
						proxy.Repositories.RDO.CreateSingle(jobHistoryDto);
					}
					catch (Exception ex)
					{
						throw new CustomExceptions.TextExtractorException(errorContext, ex);
					}
				}
			}
			catch (Exception ex)
			{
				throw new CustomExceptions.TextExtractorException(errorContext, ex);
			}
		}

		public void CreateExtractorRegularExpressionRecord(IServicesMgr svcMgr, ExecutionIdentity identity,
			Int32 workspaceArtifactId, String regExName, String regEx, String description)
		{
			var errorContext = String.Format("An error occured when creating Extractor Regular Expression Record. [WorkspaceArtifactId: {0}]", workspaceArtifactId);

			try
			{
				using (var proxy = svcMgr.CreateProxy<IRSAPIClient>(identity))
				{
					proxy.APIOptions.WorkspaceID = workspaceArtifactId;

					var jobRegExDto = new RDO();
					jobRegExDto.ArtifactTypeGuids.Add(Constant.Guids.ObjectType.ExtractorRegularExpression);
					jobRegExDto.Fields.Add(new FieldValue(Constant.Guids.Fields.ExtractorRegularExpression.Name) { Value = regExName });
					jobRegExDto.Fields.Add(new FieldValue(Constant.Guids.Fields.ExtractorRegularExpression.RegularExpression) { Value = regEx });
					jobRegExDto.Fields.Add(new FieldValue(Constant.Guids.Fields.ExtractorRegularExpression.Description) { Value = description });
					try
					{
						proxy.Repositories.RDO.CreateSingle(jobRegExDto);
					}
					catch (Exception ex)
					{
						throw new CustomExceptions.TextExtractorException(errorContext, ex);
					}
				}
			}
			catch (Exception ex)
			{
				throw new CustomExceptions.TextExtractorException(errorContext, ex);
			}
		}

		public String GetDocumentIdentifierValue(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 documentArtifactId)
		{
			var errorContext = String.Format("An error occured when querying for Document identifier value. [WorkspaceArtifactId: {0}, DocumentArtifactId: {1}]", workspaceArtifactId, documentArtifactId);
			String retVal;

			try
			{
				using (var proxy = svcMgr.CreateProxy<IRSAPIClient>(identity))
				{
					proxy.APIOptions.WorkspaceID = workspaceArtifactId;
					Document documentRdo;
					try
					{
						documentRdo = proxy.Repositories.Document.ReadSingle(Convert.ToInt32(documentArtifactId));
					}
					catch (Exception ex)
					{
						throw new CustomExceptions.TextExtractorException(errorContext + ". Document ReadSingle failed.", ex);
					}

					retVal = documentRdo.TextIdentifier;
				}
			}
			catch (Exception ex)
			{
				throw new CustomExceptions.TextExtractorException(errorContext, ex);
			}

			return retVal;
		}

		public String GetFieldNameForArtifactId(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 fieldArtifactId)
		{
			String retVal;
			var errorContext = String.Format("An error occured when querying for Field name. [WorkspaceArtifactId: {0}, FieldArtifactId: {1}]", workspaceArtifactId, fieldArtifactId);

			try
			{
				using (var proxy = svcMgr.CreateProxy<IRSAPIClient>(identity))
				{
					proxy.APIOptions.WorkspaceID = workspaceArtifactId;
					kCura.Relativity.Client.DTOs.Field fieldRdo;
					try
					{
						fieldRdo = proxy.Repositories.Field.ReadSingle(Convert.ToInt32(fieldArtifactId));
					}
					catch (Exception ex)
					{
						throw new CustomExceptions.TextExtractorException(errorContext + ". Field ReadSingle failed.", ex);
					}

					retVal = fieldRdo.Name;
				}
			}
			catch (Exception ex)
			{
				throw new CustomExceptions.TextExtractorException(errorContext, ex);
			}

			return retVal;
		}

		public String GetExtractorSetNameForArtifactId(IServicesMgr svcMgr, ExecutionIdentity identity, Int32 workspaceArtifactId, Int32 extractorSetArtifactId)
		{
			String retVal;
			var errorContext = String.Format("An error occured when querying for Extractor Set name. [WorkspaceArtifactId: {0}, ExtractorSetArtifactId: {1}]", workspaceArtifactId, extractorSetArtifactId);

			try
			{
				using (var proxy = svcMgr.CreateProxy<IRSAPIClient>(identity))
				{
					proxy.APIOptions.WorkspaceID = workspaceArtifactId;
					RDO extractorSetRdo;
					try
					{
						extractorSetRdo = proxy.Repositories.RDO.ReadSingle(Convert.ToInt32(extractorSetArtifactId));
					}
					catch (Exception ex)
					{
						throw new CustomExceptions.TextExtractorException(errorContext + ". Field ReadSingle failed.", ex);
					}

					retVal = extractorSetRdo.TextIdentifier;
				}
			}
			catch (Exception ex)
			{
				throw new CustomExceptions.TextExtractorException(errorContext, ex);
			}

			return retVal;
		}
	}
}